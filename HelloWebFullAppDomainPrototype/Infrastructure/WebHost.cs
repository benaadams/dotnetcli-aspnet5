using System;
using System.IO;

namespace Microsoft.AspNet.Hosting
{
    public static class WebHost
    {
        public static void ExecuteInChildAppDomain<TStartup>(string[] args)
        {
            ExecuteInChildAppDomain<TStartup>(Environment.CurrentDirectory, args);
        }

        public static void ExecuteInChildAppDomain<TStartup>(string applicationBasePath, string[] args)
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                var setup = new AppDomainSetup
                {
                    ApplicationBase = applicationBasePath, // Read this from somewhere sensible
                    PrivateBinPath = Path.GetDirectoryName(typeof(TStartup).Assembly.Location) // Path to where the assemblies are
                };

                var domain = AppDomain.CreateDomain("webdomain", AppDomain.CurrentDomain.Evidence, setup);
                int exitCode = domain.ExecuteAssemblyByName(typeof(TStartup).Assembly.FullName, args);
                Environment.Exit(exitCode);
            }
        }
    }
}