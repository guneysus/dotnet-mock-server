using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

public static class MockServerExtensions
{
    static IApplicationBuilder App { get; set; }

    public static IRouteBuilder Example(this IApplicationBuilder app)
    {
        RouteHandler defaultHandler = new RouteHandler(context => {
            var routeValues = context.GetRouteData().Values;
            return context.Response.WriteAsync(
                $"Hello! Route values: {string.Join(", ", routeValues)}");
        });

        RouteBuilder builder = new RouteBuilder(app, defaultHandler);

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

        return builder;
    }

    public static IRouteBuilder UseMockServer(this IApplicationBuilder app, RouteHandler defaultHandler)
    {
        App = app;
        return new RouteBuilder(App, defaultHandler);
    }

    public static RouteBuilder UseMockServer(this IApplicationBuilder app)
    {
        App = app;
        return new RouteBuilder(App);
    }

    public static void BuildRoutes(this RouteBuilder builder)
    {
        IRouter routes = builder.Build();
        App.UseRouter(routes);
    }

    static Task Json<T>(HttpContext handler, Func<T> func)
    {
        handler.Response.Headers.Add("content-type", "application/json");
        return handler.Response.WriteAsync(JsonConvert.SerializeObject(func()));

    }

    public static RouteBuilder Get<T>(this RouteBuilder builder, string url, Func<T> func)
    {
        builder.MapGet(url, handler =>
        {
            return Json(handler, func);
        });

        return builder;

    }

    public static RouteBuilder Post<T>(this RouteBuilder builder, string url, Func<T> func)
    {
        builder.MapPost(url, handler =>
        {
            return Json(handler, func);
        });

        return builder;

    }

    public static RouteBuilder Get(this RouteBuilder builder, string url, RequestDelegate handler)
    {
        builder.MapGet(url, handler);
        return builder;
    }

    public static RouteBuilder Post(this RouteBuilder builder, string url, RequestDelegate handler)
    {
        builder.MapPost(url, handler);
        return builder;
    }
}