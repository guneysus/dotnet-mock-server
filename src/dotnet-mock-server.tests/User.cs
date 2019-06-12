using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace dotnet_mock_server.tests
{
    public partial class User
    {
        [JsonProperty("_id")]
        public Guid Id { get; set; }

        [JsonProperty("birthDate")]
        public DateTimeOffset BirthDate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("company")]
        public Company Company { get; set; }

        [JsonProperty("random")]
        public Random Random { get; set; }
    }

    public class RequestInstrospectionResponse
    {
        [JsonProperty("headers")]
        public IDictionary<string, string> Headers { get; set; }

        [JsonProperty("params")]
        public IDictionary<string, string> Params { get; set; }

        [JsonProperty("form")]
        public IDictionary<string, string> Form { get; set; }
    }
}
