using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dotnet_mock_server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddRouting();
            services.AddTransient<IUserRepository, MockUserRepository>();

            //services.AddTransient<MockRepository<User>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                      ForwardedHeaders.XForwardedProto
            });


            app
                .UseMockServer()

                .On("GET", "/response/json", "application/json", "{}")

                .OnGet("/response/json", "application/json", "{}")

                .OnGet("/response/json", "application/json", (req, resp, route) =>
                {
                    return "{}";
                })

                .On("GET", "/response/xml", "application/xml", $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<note>
  <to>Tove</to>
  <from>Jani</from>
  <heading>Reminder</heading>
  <body>Don't forget me this weekend!</body>
</note>
")
            .On("GET", "/api/user", "application/json", JsonConvert.SerializeObject(app.ApplicationServices.GetService<IUserRepository>().GetAllUsers()))

            .On("GET", "/api/user/1", "application/json", JsonConvert.SerializeObject(app.ApplicationServices.GetService<IUserRepository>()[1]))

            .OnGet<User>("/api/user/{id:int}", "application/json", (route) =>
            {
                var id = Convert.ToInt32(route.Values["id"]);
                var repo = app.ApplicationServices.GetService<IUserRepository>();
                var user = repo[id];
                return user;
            })

            .OnGet("/api/user/{id:int}", "application/json", (route) =>
            {
                var id = Convert.ToInt32(route.Values["id"]);
                var repo = app.ApplicationServices.GetService<IUserRepository>();
                var user = repo[id];
                var content = JsonConvert.SerializeObject(user);
                return content;
            })

            .OnGet<User>("/api/user/{id:int}", "application/json", (route) =>
            {
                var id = Convert.ToInt32(route.Values["id"]);
                var repo = app.ApplicationServices.GetService<IUserRepository>();
                var user = repo[id];
                return user;
            })

            .BuildRoutes();
        }
    }
}

public class MockUserRepository : IUserRepository
{
    private List<User> _users = JsonConvert.DeserializeObject<List<User>>(@"
[{""id"":""1"",""createdAt"":""2019-05-06T19:32:07.034Z"",""name"":""Rosario Beier"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/prrstn/128.jpg""},{""id"":""2"",""createdAt"":""2019-05-07T10:28:32.699Z"",""name"":""Rocio Gibson DVM"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/SlaapMe/128.jpg""},{""id"":""3"",""createdAt"":""2019-05-06T17:00:24.825Z"",""name"":""Adeline Torphy"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/catadeleon/128.jpg""}]
");

    public User this[int index] => _users.FirstOrDefault(x => x.Id == index);

    public List<User> GetAllUsers() => _users;

}

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

public interface IUserRepository
{
    User this[int index] { get; }

    List<User> GetAllUsers();
}