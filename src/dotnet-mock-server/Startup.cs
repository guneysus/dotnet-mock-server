using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NMock;
using System;
using System.IO;
using System.Linq;

namespace NMock
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                      ForwardedHeaders.XForwardedProto
            });

            var configFile = Configuration.GetValue<string>("config", "mockServer.json");

            MockServerConfig mockConfig = MockServerConfig.ReadConfig(configFile);
            app.UseMockServer(mockConfig);
        }
    }
}
