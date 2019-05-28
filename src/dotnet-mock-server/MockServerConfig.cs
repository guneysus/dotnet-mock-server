using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

[JsonDictionary]
public partial class MockServerConfig : Dictionary<string, MockServerUrlConfig>
{

    public static MockServerConfig ReadConfig(string path)
    {
        var configFileInfo = new FileInfo(path);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Config file is not found", configFileInfo.FullName);
        }

        var content = File.ReadAllText(configFileInfo.FullName);
        var config = JsonConvert.DeserializeObject<MockServerConfig>(content);
        return config;
    }
}
