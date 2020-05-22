using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace NMock.tests
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

        [Fact(DisplayName = "/json")]
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
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("/redirect");
            Assert.Equal(301, (int)response.StatusCode);

            HttpContentHeaders contentHeaders = response.Content.Headers;
            HttpResponseHeaders headers = response.Headers;

            Assert.NotEmpty(headers.Location.OriginalString);
        }

        [Fact(DisplayName = "/api/comment")]
        public async Task get_comments()
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

        [Fact(DisplayName = "/request")]
        public async Task request_instrospection()
        {
            client.DefaultRequestHeaders.Add("X-Foo", "Bar");


            var form = new Dictionary<string, string>() {
                    {"foo", "bar" }
                };

            var content = new FormUrlEncodedContent(form.ToList());

            var httpResponseMessage = await client.PostAsync("/request/3444?result=ok", content); 

            httpResponseMessage.EnsureSuccessStatusCode();

            var body = await httpResponseMessage.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<RequestInstrospectionResponse>(body);

            Assert.NotNull(response);
            Assert.NotEqual(default(Dictionary<string, IEnumerable<string>>), response.Headers);
            Assert.NotEqual(default(Dictionary<string, IEnumerable<string>>), response.Query);
            Assert.NotEqual(default(Dictionary<string, IEnumerable<string>>), response.Form);
            Assert.NotEqual(default(Dictionary<string, string>), response.Route);

        }
    }
}
