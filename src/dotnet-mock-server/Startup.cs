using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                      ForwardedHeaders.XForwardedProto
            });

            app
                .UseMockServer()

                .Get("now", () => DateTime.Now)
                .Post("now", () => DateTime.Now)

                .Get("ip", h =>
                {
                    return h.Response.WriteAsync(h.Connection.RemoteIpAddress.ToString());
                })
                .Post("ip", h =>
                {
                    return h.Response.WriteAsync(h.Connection.RemoteIpAddress.ToString());
                })

                .Get("/api/user", () =>
                {
                    List<User> response = GetAllUsers();
                    return response;
                })
                .Get("/api/user/{id:int}", handler =>
                {
                    int id = Convert.ToInt32(handler.GetRouteValue("id"));
                    var users = GetAllUsers();
                    User user = users.First(x => x.Id == id);
                    return handler.Response.WriteAsync(JsonConvert.SerializeObject(user));
                })

                .BuildRoutes()
                ;
        }

        private static List<User> GetAllUsers()
        {
            return JsonConvert.DeserializeObject<List<User>>(@"
[{""id"":""1"",""createdAt"":""2019-05-06T19:32:07.034Z"",""name"":""Rosario Beier"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/prrstn/128.jpg""},{""id"":""2"",""createdAt"":""2019-05-07T10:28:32.699Z"",""name"":""Rocio Gibson DVM"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/SlaapMe/128.jpg""},{""id"":""3"",""createdAt"":""2019-05-06T17:00:24.825Z"",""name"":""Adeline Torphy"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/catadeleon/128.jpg""}]
");
        }
    }
}

public partial class User
{
    [JsonProperty("id")]
    //[JsonConverter(typeof(ParseStringConverter))]
    public long Id { get; set; }

    [JsonProperty("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("avatar")]
    public Uri Avatar { get; set; }
}