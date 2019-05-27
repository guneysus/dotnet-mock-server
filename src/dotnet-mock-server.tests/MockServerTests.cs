using Newtonsoft.Json;
using System.Collections.Generic;
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

        [Fact]
        public async Task get_json_async()
        {
            // Act
            var response = await client.GetAsync("/json");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Contains("application/json",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task get_xml_async()
        {
            // Act
            var response = await client.GetAsync("/xml");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Contains("application/xml",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task get_users_async()
        {
            var response = await client.GetAsync("/api/user/");
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var content = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(content);

            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task get_single_user_async()
        {
            var response = await client.GetAsync("/api/user/1");
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var content = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);

            Assert.NotNull(user);
        }

        [Fact]
        public async Task check_headers()
        {
            // Act
            var response = await client.GetAsync("/headers");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var content = await response.Content.ReadAsStringAsync();

            HttpResponseHeaders headers = response.Headers;

            Assert.Equal(content.Length, response.Content.Headers.ContentLength);
            Assert.Equal("bar", headers.GetValues("foo").First());

        }
    }
}
