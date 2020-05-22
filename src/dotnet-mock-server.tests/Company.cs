using Newtonsoft.Json;
using System;

namespace NMock.tests
{
    public partial class Company
    {
        [JsonProperty("_id")]
        public Guid Id { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }
    }
}
