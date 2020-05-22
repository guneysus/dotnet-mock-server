using Newtonsoft.Json;

namespace NMock.tests
{
    public static class Serialize
    {
        public static string ToJson(this Comment self) => JsonConvert.SerializeObject(self);
    }
}
