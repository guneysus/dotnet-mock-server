using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class MockServerExtensions
{
    static IApplicationBuilder App { get; set; }

    public static IRouteBuilder UseMockServer(this IApplicationBuilder app, MockServerConfig config)
    {
        App = app;
        RouteBuilder routeBuilder = new RouteBuilder(App);

        foreach (KeyValuePair<string, MockServerUrlConfig> kv in config)
        {
            var url = kv.Key;
            var urlConfig = kv.Value;

            foreach (var ukv in urlConfig)
            {
                var method = ukv.Key;
                MockServerHttpVerb verb = ukv.Value;

                routeBuilder.On(method, url, verb);
            }

        }

        routeBuilder.BuildAndUseRouter();

        return routeBuilder;
    }

    public static IRouteBuilder UseMockServer(this IApplicationBuilder app)
    {
        App = app;
        return new RouteBuilder(App);
    }

    public static void BuildAndUseRouter(this IRouteBuilder builder)
    {
        IRouter routes = builder.Build();
        App.UseRouter(routes);
    }

    static Task Content(HttpResponse response, MockServerHttpVerb verb)
    {
        response.Headers.Add("content-type", verb.ContentType);

        foreach (var kv in verb.Headers)
        {
            response.Headers.Add(kv.Key, kv.Value);
        }

        Task task;

        if (verb.Content != null)
        {
            response.ContentLength = verb.Content.ToString().Length;
            response.StatusCode = verb.StatusCode;
            task = response.WriteAsync(verb.Content.ToString());
        }
        else
        {
            response.ContentLength = 0;
            response.StatusCode = verb.StatusCode;
            task = response.WriteAsync(string.Empty);
        }

        return task;
    }

    static Task Json<T>(HttpContext handler, Func<T> func)
    {
        HttpResponse response = handler.Response;

        string json = JsonConvert.SerializeObject(func());
        
        return Json(response, json);
    }

    static Task Json<T>(HttpContext handler, T entity)
    {
        HttpResponse response = handler.Response;
        string json = JsonConvert.SerializeObject(entity);
        return Json(response, json);
    }

    static Task Json<T>(HttpResponse response, T entity)
    {
        string json = JsonConvert.SerializeObject(entity);
        return Json(response, json);
    }

    public static IRouteBuilder On(this IRouteBuilder builder,
        string method,
        string template,
        string contentType,
        object content, Dictionary<string, string> headers = null, int statusCode = 200)
    {
        var verb = new MockServerHttpVerb()
        {
            Content = content,
            ContentType = contentType,
            Headers = headers ?? new Dictionary<string, string>(),
            StatusCode = statusCode
        };

        builder.MapVerb(method, template, (req, resp, route) =>
        {
            return Content(resp, verb);
        });

        return builder;
    }

    public static IRouteBuilder On(this IRouteBuilder builder,
        string method,
        string template, MockServerHttpVerb verb)
    {
        builder.MapVerb(method, template, (req, resp, route) =>
        {
            return Content(resp, verb);
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
            var verb = new MockServerHttpVerb()
            {
                ContentType = contentType,
                Content = content
            };

            return Content(resp, verb);
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
            string content = gen(req, resp, route);
            var verb = new MockServerHttpVerb()
            {
                ContentType = contentType,
                Content = content
            };

            return Content(resp, verb);
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
            string content = JsonConvert.SerializeObject(body);

            var verb = new MockServerHttpVerb()
            {
                ContentType = contentType,
                Content = content
            };

            return Content(resp, verb);
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
            string content = gen(route);

            var verb = new MockServerHttpVerb()
            {
                ContentType = contentType,
                Content = content
            };

            return Content(resp, verb);
        });

        return builder;
    }
}