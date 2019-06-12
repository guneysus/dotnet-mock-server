using Newtonsoft.Json;

namespace dotnet_mock_server.tests
{
    public static class Serialize
    {
        public static string ToJson(this Comment self) => JsonConvert.SerializeObject(self);
    }
}
