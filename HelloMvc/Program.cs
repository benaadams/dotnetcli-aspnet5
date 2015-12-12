using Microsoft.AspNet.Hosting;

namespace HelloMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplication.Run<Startup>(args);
        }
    }
}