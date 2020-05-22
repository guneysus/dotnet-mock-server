using Newtonsoft.Json;
using System;

namespace NMock.tests
{
    public partial class Random
    {
        [JsonProperty("number")]
        public long Number { get; set; }

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("bool")]
        public bool Bool { get; set; }

        [JsonProperty("alpha")]
        public string Alpha { get; set; }

        [JsonProperty("alphaNumeric")]
        public string AlphaNumeric { get; set; }

        [JsonProperty("numeric")]
        public string Numeric { get; set; }

        [JsonProperty("pattern")]
        public string Pattern { get; set; }
    }
}
