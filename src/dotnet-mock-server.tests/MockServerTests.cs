using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace dotnet_mock_server.tests
{
    public class MockServerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private HttpClient client;

        public MockServerTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            client = factory.CreateClient();
        }

        [Fact(DisplayName ="/json")]
        public async Task get_json()
        {
            // Act
            var response = await client.GetAsync("/json");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Contains("application/json",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact(DisplayName = "/xml")]
        public async Task get_xml()
        {
            // Act
            var response = await client.GetAsync("/xml");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Contains("application/xml",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact(DisplayName = "/api/user")]
        public async Task get_users()
        {
            var response = await client.GetAsync("/api/user/");
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var content = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(content);

            Assert.NotEmpty(users);
        }

        [Fact(DisplayName = "/api/user/1000")]
        public async Task get_single_user()
        {
            var response = await client.GetAsync("/api/user/1000");
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);

            Assert.NotNull(user);
        }

        [Fact(DisplayName = "/headers")]
        public async Task check_headers()
        {
            // Act
            var response = await client.GetAsync("/headers");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var content = await response.Content.ReadAsStringAsync();

            HttpResponseHeaders headers = response.Headers;

            Assert.Equal(content.Length, response.Content.Headers.ContentLength);
            Assert.Equal("MockServer.NET", headers.GetValues("X-Powered-By").First());

        }

        [Fact(DisplayName = "/redirect")]
        public async Task check_redirect()
        {
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions() {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/redirect");
            Assert.Equal(301, (int)response.StatusCode);

            HttpContentHeaders contentHeaders = response.Content.Headers;
            HttpResponseHeaders headers = response.Headers;

            Assert.NotEmpty(headers.Location.OriginalString);
        }

        [Fact(DisplayName ="/api/comment")]
        public async Task get_comments ()
        {
            var response = await client.GetAsync("/api/comment");
            response.EnsureSuccessStatusCode();


            var body = await response.Content.ReadAsStringAsync();
            
            var data = JsonConvert.DeserializeObject<RestArrayData<Comment>>(body);

            Assert.NotEmpty(data.Objects);
            Assert.NotEqual(Guid.Empty, data.Objects.First().Id);
        }

        [Fact(DisplayName = "/api/comment/9999")]
        public async Task get_single_comment()
        {
            var response = await client.GetAsync("/api/comment/9999");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var comment = JsonConvert.DeserializeObject<Comment>(body);

            Assert.NotNull(comment);
            Assert.NotEqual(Guid.Empty, comment.Id);

            Assert.NotEqual("$lorem.sentence", comment.Title);
            Assert.NotEqual("$lorem.paragraph", comment.Text);

        }

        [Fact(DisplayName = "/fake/comment/1000")]
        public async Task get_fake_comment()
        {
            var response = await client.GetAsync("/fake/comment/1000");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var comment = JsonConvert.DeserializeObject<Comment>(body);

            Assert.NotNull(comment);
            Assert.NotEqual(Guid.Empty, comment.Id);

            Assert.NotEqual("$lorem.sentence", comment.Title);
            Assert.NotEqual("$lorem.paragraph", comment.Text);

        }
    }

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

    public partial class Company
    {
        [JsonProperty("_id")]
        public Guid Id { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }
    }

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

    public static class Serialize
    {
        public static string ToJson(this Comment self) => JsonConvert.SerializeObject(self);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }

            return value;

            //throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}


public class RestArrayData<T>
{
    public IEnumerable<T> Objects { get; set; }
}