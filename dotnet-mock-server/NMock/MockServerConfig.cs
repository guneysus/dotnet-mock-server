using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace NMock
{
    public partial class MockServerConfig
    {
        [JsonProperty("templates", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Dictionary<string, object> Templates { get; set; }

        public static string DefaultConfigFileFullpath { get; set; }

        [JsonProperty("resources")]
        public Dictionary<string, MockServerUrlConfig> Resources { get; set; }

        public static MockServerConfig ReadConfig(string path)
        {
            FileInfo configFileInfo = CheckAndGenerateConfig(path);

            var content = File.ReadAllText(configFileInfo.FullName);
            var config = JsonConvert.DeserializeObject<MockServerConfig>(content, new JsonConverter[] { });

            instance = config;

            HR("Resources");


            Console.WriteLine($"GET /__config");

            config.Resources.ToList().ForEach(res =>
            {
                foreach (var verb in res.Value.Keys)
                {
                    Console.WriteLine($"{verb.ToUpper()} {res.Key}");
                }
            });

            HR("Resources");


            return config;
        }

        private static void HR(string title, int cols = 80)
        {
            var len = (cols - title.Length) / 2;

            string spacer = string.Join(string.Empty, Enumerable.Repeat("-", len).Select(x => "-"));
            Console.Write(spacer);
            Console.Write($" {title}");
            Console.Write(spacer);
            Console.WriteLine();

        }

        private static FileInfo CheckAndGenerateConfig(string path)
        {
            var configFileInfo = new FileInfo(path);

            if (!File.Exists(path))
            {
                GenerateDefaultConfig(path);
                FileInfo fileInfo = new FileInfo(path);
                return fileInfo;
                //throw new FileNotFoundException("Config file is not found", configFileInfo.FullName);
            }

            return configFileInfo;
        }

        private MockServerConfig()
        {
        }

        static MockServerConfig() => DefaultConfigFileFullpath = Path.Join(AppContext.BaseDirectory, "template.json");

        static MockServerConfig instance = null;

        public static MockServerConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MockServerConfig();
                }
                return instance;
            }
        }

        public static void GenerateDefaultConfig(string path)
        {
            File.Copy(DefaultConfigFileFullpath, path, overwrite: true);
        }
    }
}