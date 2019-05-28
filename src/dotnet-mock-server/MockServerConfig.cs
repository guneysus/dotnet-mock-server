using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

[JsonDictionary]
public partial class MockServerConfig : Dictionary<string, MockServerUrlConfig>
{

    public static MockServerConfig ReadConfig(string path)
    {
        var content = File.ReadAllText(path);
        var config = JsonConvert.DeserializeObject<MockServerConfig>(content);
        return config;
    }
}
