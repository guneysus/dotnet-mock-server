using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

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

[JsonDictionary]
public partial class MockConfig : Dictionary<string, UrlConfig>
{

    public static MockConfig ReadConfig(string path)
    {
        var content = File.ReadAllText(path);
        var config = JsonConvert.DeserializeObject<MockConfig>(content);
        return config;
    }
}

public partial class MockConfig : Dictionary<string, UrlConfig>
{
}

public partial class UrlConfig : Dictionary<string, Verb>
{
}

public class Verb
{
    [JsonProperty("content")]
    public object Content { get; set; }

    [JsonProperty("contentType")]
    public string ContentType { get; set; }

    [JsonProperty("headers")]
    public Dictionary<string, string> Headers { get; set; }
}