using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


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

        return config;
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

    static MockServerConfig()
    {
        DefaultConfigFileFullpath = Path.Join(AppContext.BaseDirectory, "configTemplate.json");
    }

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