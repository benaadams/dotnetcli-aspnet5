using Microsoft.Extensions.CompilationAbstractions;

namespace HelloMvc
{
    public class NullExporter : ILibraryExporter
    {
        public LibraryExport GetAllExports(string name)
        {
            return null;
        }

        public LibraryExport GetExport(string name)
        {
            return null;
        }
    }
}