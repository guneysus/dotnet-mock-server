using Newtonsoft.Json;
using System;

namespace dotnet_mock_server.tests
{
    public partial class Comment
    {
        [JsonProperty("_id")]
        public Guid Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }
}
