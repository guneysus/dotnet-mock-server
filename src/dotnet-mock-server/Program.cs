using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Runtime.InteropServices;

namespace NMock
{
    public static class Program
    {
        public static void Main(string[] args)
        {

            #region CLI Argument parser
            // Create some options and a parser
            var generateConfigOption = new Option(
                "--generate-config",
                "To generate an example `mockServer.json` config, pass this parameter",
                new Argument<bool>(defaultValue: false));


            string DEFAULT_CONFIG_PATH = Path.Join(Directory.GetCurrentDirectory(), "mockServer.json");

            var configOption = new Option(
                "--config",
                "Config file path",
                new Argument<string>(DEFAULT_CONFIG_PATH));

            var urlsOption = new Option("--urls", "" +
                "Set ASP.Net Core URL: Example: --urls http://+:3005",
                new Argument<string>("http://localhost:3005"));


            // Add them to the root command
            var rootCommand = new RootCommand();
            rootCommand.Description = "Mock Server";

            rootCommand.AddOption(generateConfigOption);
            rootCommand.AddOption(configOption);
            rootCommand.AddOption(urlsOption);

            rootCommand.Handler = CommandHandler.Create<bool, string>((generateConfig, config) =>
            {
                if (config == "mockServer.json")
                {
                    throw new ArgumentException(nameof(config));
                }

                var configTemplate = new FileInfo(DEFAULT_CONFIG_PATH).FullName;

                Console.WriteLine($"--generate-config => {generateConfig}");

                if (!File.Exists(config))
                {
                    Console.WriteLine($"Generating default config => {new FileInfo(config).FullName}");
                    generateConfig = true;
                }

                if (generateConfig)
                {
                    MockServerConfig.GenerateDefaultConfig(config);
                }


                #endregion

                #region web
                IWebHostBuilder webHostBuilder = CreateWebHostBuilder(args);
                IWebHost webHost = webHostBuilder.Build();

                webHost.Run();
                #endregion

            });

            // Parse the incoming args and invoke the handler
            rootCommand.InvokeAsync(args).Wait();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseLibuv()
                .UseKestrel()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);

                })
                .UseStartup<Startup>();
        }
    }
}
