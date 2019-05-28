using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace dotnet_mock_server
{
    public class Program
    {
        private const string DEFAULT_CONFIG_FILE = "configTemplate.json";

        public static void Main(string[] args)
        {

            #region CLI Argument parser
            // Create some options and a parser
            var generateConfigOption = new Option(
                "--generate-config",
                "An option whose argument is parsed as an int",
                new Argument<bool>(defaultValue: false));

            var configOption = new Option(
                "--config",
                "An option whose argument is parsed as an int",
                new Argument<string>("mockServer.json"));

            // Add them to the root command
            var rootCommand = new RootCommand();
            rootCommand.Description = "Mock Server";

            rootCommand.AddOption(generateConfigOption);
            rootCommand.AddOption(configOption);

            rootCommand.Handler = CommandHandler.Create<bool, string>((generateConfig, config) =>
            {
                if (config == DEFAULT_CONFIG_FILE)
                    throw new ArgumentException(nameof(config));

                //var configTemplate = new FileInfo(DEFAULT_CONFIG_FILE).FullName;

                Console.WriteLine($"--generate-config => {generateConfig}");
                
                if(generateConfig)
                {
                    File.Copy(DEFAULT_CONFIG_FILE, config, overwrite: true);
                }
            });

            // Parse the incoming args and invoke the handler
            rootCommand.InvokeAsync(args).Wait();

            #endregion

            #region web
            IWebHostBuilder webHostBuilder = CreateWebHostBuilder(args);
            IWebHost webHost = webHostBuilder.Build();

            webHost.Run();
            #endregion
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
