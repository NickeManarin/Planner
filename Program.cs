using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Planner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();

            //TODO: Replace with your local address.
            //webBuilder.UseUrls("http://192.168.0.9:5000", "https://192.168.0.9:5001"); 
        });
    }
}