using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class HelloRouterExtensions
{

    public static IApplicationBuilder UseHelloRouter(this IApplicationBuilder app)
    {
        RouteHandler trackPackageRouteHandler = new RouteHandler(context => {
            var routeValues = context.GetRouteData().Values;
            return context.Response.WriteAsync(
                $"Hello! Route values: {string.Join(", ", routeValues)}");
        });

        RouteBuilder builder = new RouteBuilder(app, trackPackageRouteHandler);

        builder.MapRoute(
            "Track Package Route",
            "package/{operation:regex(^track|create|detonate$)}/{id:int}");

        builder.MapGet("hello/{name}",handler => {
            object name = handler.GetRouteValue("name");
            // This is the route handler when HTTP GET "hello/<anything>"  matches
            // To match HTTP GET "hello/<anything>/<anything>,
            // use routeBuilder.MapGet("hello/{*name}"
            return handler.Response.WriteAsync($"Hi, {name}!");
        });

        IRouter routes = builder.Build();

        app.UseRouter(routes);
        return app;
    }
}