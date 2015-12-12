using Microsoft.AspNet.Hosting;

namespace HelloWeb
{
    public class Program
    {
		public static void Main(string[] args)
		{
			WebApplication.Run<Startup>(args);
		}	
	}
}