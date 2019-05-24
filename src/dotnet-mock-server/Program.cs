using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace dotnet_mock_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHostBuilder webHostBuilder = CreateWebHostBuilder(args);
            IWebHost webHost = webHostBuilder.Build();
            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseLibuv()
                .UseKestrel()
                .UseStartup<Startup>();
    }
}
