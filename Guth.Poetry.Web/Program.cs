
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
namespace Guth.Poetry.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseStartup<Startup>();
                })
                .Build()
                .Run();
        }
    }
}
