using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.CompilationAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace HelloMvc
{
    public class Startup
    {
        private readonly IApplicationEnvironment _applicationEnvironment;

        public Startup(IApplicationEnvironment applicationEnvironment)
        {
            _applicationEnvironment = applicationEnvironment;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // We need to disable register an empty exporter so that we can add references
            // This will go away eventually see
            services.AddSingleton<ILibraryExporter, NullExporter>();

            // ILibraryManager isn't available yet so we need to explicitly add the assemblies
            // that we want to find controllers in. 
            var assemblyProvider = new StaticAssemblyProvider();
            assemblyProvider.CandidateAssemblies.Add(typeof(Startup).GetTypeInfo().Assembly);
            services.AddSingleton<IAssemblyProvider>(assemblyProvider);

            services.AddMvc()
                    .AddRazorOptions(options =>
                    {
                        options.CompilationCallback = c =>
                        {
                            var refs = ResolveCompilationReferences();

                            c.Compilation = c.Compilation
                                .AddReferences(refs)
                            ;
                        };
                    });
        }

        private IEnumerable<MetadataReference> ResolveCompilationReferences()
        {
            // See the following issues for progress on resolving references:
            // https://github.com/aspnet/Mvc/issues/3633
            // https://github.com/dotnet/cli/issues/376
            
            // HACK WARNING: We don't have a way to get the reference assemblies at runtime
            // so this is super hacky and parses the response file passed into dotnet-compile-csc in order to discover 
            // compile time references. It's VERY fragile and barely works :).
            var responseFileName = $"dotnet-compile.{_applicationEnvironment.ApplicationName}.rsp";
            var baseDir = new DirectoryInfo(AppContext.BaseDirectory);
            string responseFilePath = Path.Combine(baseDir.FullName, responseFileName);

            if (!File.Exists(responseFilePath))
            {
                responseFilePath = Path.Combine(baseDir.Parent.Parent.Parent.FullName, "obj", baseDir.Parent.Name, baseDir.Name, responseFileName);
            }

            if (!File.Exists(responseFilePath))
            {
                // Logic was too flaky
                throw new InvalidOperationException("Unable to resolve references!");
            }

            // Parse line by line and find the references
            var refs = File.ReadAllLines(responseFilePath)
                           .Where(l => l.StartsWith("--reference:"))
                           .Select(l => l.Substring("--reference:".Length))
                           .Select(path => MetadataReference.CreateFromFile(path));
            return refs;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}