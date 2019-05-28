using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

public class MockServerHttpVerb
{
    [JsonProperty("content")]
    public object Content { get; set; }

    [JsonProperty("contentType")]
    public string ContentType { get; set; }

    [JsonProperty("headers")]
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    [JsonProperty("statusCode")]
    public int StatusCode { get; set; } = (int)HttpStatusCode.OK;

    [JsonProperty("dynamic")]
    public bool Dynamic { get; set; }

    // TODO Use HttpStatusCode enum.
}