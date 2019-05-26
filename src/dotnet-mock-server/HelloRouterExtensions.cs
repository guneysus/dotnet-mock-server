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

    public static IRouteBuilder UseMockServer(this IApplicationBuilder app)
    {
        App = app;
        return new RouteBuilder(App);
    }

    public static void BuildRoutes(this IRouteBuilder builder)
    {
        IRouter routes = builder.Build();
        App.UseRouter(routes);
    }

    static Task Json<T>(HttpContext handler, Func<T> func)
    {
        handler.Response.Headers.Add("content-type", "application/json");
        return handler.Response.WriteAsync(JsonConvert.SerializeObject(func()));
    }

    static Task Json<T>(HttpContext handler, T entity)
    {
        handler.Response.Headers.Add("content-type", "application/json");
        return handler.Response.WriteAsync(JsonConvert.SerializeObject(entity));
    }

    public static IRouteBuilder Get<T>(this IRouteBuilder builder, string url, Func<T> func)
    {
        builder.MapGet(url, handler =>
        {
            return Json(handler, func);
        });

        return builder;
    }

    public static IRouteBuilder Get<T>(this IRouteBuilder builder, string url, T entity)
    {
        builder.MapGet(url, handler =>
        {
            return Json(handler, entity);
        });

        return builder;
    }

    public static IRouteBuilder Post<T>(this IRouteBuilder builder, string url, Func<T> func)
    {
        builder.MapPost(url, handler =>
        {
            return Json(handler, func);
        });

        return builder;

    }

    public static IRouteBuilder Get(this IRouteBuilder builder, string url, RequestDelegate handler)
    {
        builder.MapGet(url, handler);
        return builder;
    }

    public static IRouteBuilder Get<T>(this IRouteBuilder builder, string url, RequestDelegate handler)
    {
        builder.MapGet(url, handler);
        return builder;
    }

    public static IRouteBuilder Post(this IRouteBuilder builder, string url, RequestDelegate handler)
    {
        builder.MapPost(url, handler);
        return builder;
    }

    public static IRouteBuilder On(this IRouteBuilder builder,
        string method,
        string template,
        string contentType,
        string content)
    {
        builder.MapVerb(method, template, (req, resp, route) =>
        {
            resp.Headers.Add("content-type", contentType);
            resp.Headers.ContentLength = content.Length;
            return resp.WriteAsync(content);
        });

        return builder;
    }

    public static IRouteBuilder OnGet(this IRouteBuilder builder,
        string template,
        string contentType,
        string content)
    {
        builder.MapGet(template, (req, resp, route) =>
        {
            resp.Headers.Add("content-type", contentType);
            resp.Headers.ContentLength = content.Length;
            return resp.WriteAsync(content);
        });

        return builder;
    }

    public static IRouteBuilder OnGet(this IRouteBuilder builder,
        string template,
        string contentType,
        Func<HttpRequest, HttpResponse, RouteData, string> gen)
    {
        builder.MapGet(template, (req, resp, route) =>
        {
            var body = gen(req, resp, route);

            resp.Headers.Add("content-type", contentType);
            resp.Headers.ContentLength = body.Length;
            return resp.WriteAsync(body);
        });

        return builder;
    }

    public static IRouteBuilder OnGet<T>(this IRouteBuilder builder,
        string template,
        string contentType,
        Func<RouteData, T> gen)
    {
        builder.MapGet(template, (req, resp, route) =>
        {
            var body = gen(route);
            var json = JsonConvert.SerializeObject(body);
            resp.Headers.Add("content-type", contentType);
            resp.Headers.ContentLength = json.Length;
            return resp.WriteAsync(json);
        });

        return builder;
    }

    public static IRouteBuilder OnGet(this IRouteBuilder builder,
        string template,
        string contentType,
        Func<RouteData, string> gen)
    {
        builder.MapGet(template, (req, resp, route) =>
        {
            var body = gen(route);

            resp.Headers.Add("content-type", contentType);
            resp.Headers.ContentLength = body.Length;
            return resp.WriteAsync(body);
        });

        return builder;
    }
}