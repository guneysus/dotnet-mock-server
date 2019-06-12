using Newtonsoft.Json;
using System;

namespace dotnet_mock_server.tests
{
    public partial class Company
    {
        [JsonProperty("_id")]
        public Guid Id { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }
    }
}
