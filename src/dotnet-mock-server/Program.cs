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
        private const string CONFIG_FILE_NAME = "mockServer.json";

        public static void Main(string[] args)
        {

            #region CLI Argument parser
            // Create some options and a parser
            var generateConfigOption = new Option(
                "--generate-config",
                "To generate an example `mockServer.json` config, pass this parameter",
                new Argument<bool>(defaultValue: false));


            var defaultConfigFileFullPath = Path.Join(Directory.GetCurrentDirectory(), CONFIG_FILE_NAME);


            var configOption = new Option(
                "--config",
                "Config file path",
                new Argument<string>(defaultConfigFileFullPath));

            // Add them to the root command
            var rootCommand = new RootCommand();
            rootCommand.Description = "Mock Server";

            rootCommand.AddOption(generateConfigOption);
            rootCommand.AddOption(configOption);

            rootCommand.Handler = CommandHandler.Create<bool, string>((generateConfig, config) =>
            {
                if (config == CONFIG_FILE_NAME)
                {
                    throw new ArgumentException(nameof(config));
                }

                //var configTemplate = new FileInfo(DEFAULT_CONFIG_FILE).FullName;

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
