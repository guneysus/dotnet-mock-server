using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;


public partial class MockServerConfig
{
    [JsonProperty("templates", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public Dictionary<string, object> Templates { get; set; }

    [JsonProperty("resources")]
    public Dictionary<string, MockServerUrlConfig> Resources { get; set; }

    public static MockServerConfig ReadConfig(string path)
    {
        var configFileInfo = new FileInfo(path);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Config file is not found", configFileInfo.FullName);
        }

        var content = File.ReadAllText(configFileInfo.FullName);
        var config = JsonConvert.DeserializeObject<MockServerConfig>(content, new JsonConverter[] {});

        instance = config;

        return config;
    }

    private MockServerConfig()
    {

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
}