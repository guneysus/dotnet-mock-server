using Newtonsoft.Json;

namespace NMock.tests
{
    public partial class Address
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("zipcode")]
        //[JsonConverter(typeof(ParseStringConverter))]
        //public long Zipcode { get; set; }
        public string Zipcode { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }
    }
}
