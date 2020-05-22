using Newtonsoft.Json;
using System;

namespace NMock
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("avatar")]
        public Uri Avatar { get; set; }
    }
}