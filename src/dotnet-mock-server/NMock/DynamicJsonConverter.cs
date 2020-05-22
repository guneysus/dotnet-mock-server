using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NMock
{

    /// <summary>
    /// Recursive Converter
    /// https://stackoverflow.com/a/6417753/1766716
    /// </summary>
    public class DynamicJsonConverter : CustomCreationConverter<IDictionary<string, object>>
    {
        private readonly HttpRequest httpRequest;
        private readonly HttpResponse httpResponse;
        private readonly RouteData routeData;

        public DynamicJsonConverter(HttpRequest httpRequest, HttpResponse httpResponse, RouteData routeData)
        {
            this.httpRequest = httpRequest;
            this.httpResponse = httpResponse;
            this.routeData = routeData;
        }

        public override IDictionary<string, object> Create(Type objectType)
        {
            return new Dictionary<string, object>();
        }

        public override bool CanConvert(Type objectType)
        {
            // in addition to handling IDictionary<string, object>
            // we want to handle the deserialization of dict value
            // which is of type object
            return objectType == typeof(object) || base.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            if (reader.Path.StartsWith("$") && (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Null))
            {

                // TODO Refactor this.
                // Create a method DynamicCompile -> template to JSON

                object o = base.ReadJson(reader, objectType, existingValue, serializer);
                Dictionary<string, object> t = ((IEnumerable<KeyValuePair<string, object>>)o).ToDictionary(kv => kv.Key, kv => kv.Value);

                var count = Convert.ToInt32(t.GetValueOrDefault("count", 0));
                var templateName = reader.Path;
                object template = MockServerConfig.Instance.Templates[templateName];

                var items = new List<object>();

                for (int i = 0; i < count; i++)
                {
                    var item = MockServerExtensions.CompileTemplate(template.ToString());

                    items.Add(item);
                }

                return items;
            }

            if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Null)
            {
                object o = base.ReadJson(reader, objectType, existingValue, serializer);
                return o;
            }

            // if the next token is not an object
            // then fall back on standard deserializer (strings, numbers etc.)
            object obj = serializer.Deserialize(reader);
            object val = Parse(obj);
            return val;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            base.WriteJson(writer, value, serializer);
        }

        protected object Parse(object obj)
        {
            string key = obj.ToString();

            if (key == "$request.headers")
            {
                return httpRequest.Headers.ToDictionary(x => x.Key, x => x.Value);
            }

            if (key == "$request.query")
            {
                return httpRequest.Query.ToDictionary(x => x.Key, x => x.Value);
            }

            if (key == "$request.form")
            {
                //if (httpRequest.Form == null)
                //{
                //    httpResponse.ContentType = "text";

                //    return new Dictionary<string, string>();
                //}

                return httpRequest.Form.ToDictionary(x => x.Key, x => x.Value);
            }

            if (key == "$route")
            {
                return routeData.Values.ToDictionary(x => x.Key, x => x.Value);
            }

            if (key.StartsWith("$request.form."))
            {
                var propName = key.Replace("$request.form.", string.Empty);
                return httpRequest.Form[propName];
            }

            if (key.StartsWith("$route."))
            {
                var propName = key.Replace("$route.", string.Empty);
                return routeData.Values[propName];
            }

            if (key.StartsWith("$request.query."))
            {
                var propName = key.Replace("$request.query.", string.Empty);
                return httpRequest.Query[propName];
            }

            Func<object> fn;

            if (Fn.TryGetValue(key, out fn))
            {
                return fn();
            }

            return obj;
        }

        public Dictionary<string, Func<object>> Fn { get; set; } = new Dictionary<string, Func<object>>()
    {
        {"$guid", new Func<string>(()=> Guid.NewGuid().ToString() ) },
        {"$name", new Func<string>(()=> Faker.Internet.UserName() ) },
        {"$fullname", new Func<string>(()=> Faker.Name.FullName() ) },
        {"$email", new Func<string>(()=> Faker.Internet.Email()) },
        {"$userName", new Func<string>(()=> Faker.Internet.UserName() ) },
        {"$phoneNumber", new Func<string>(()=> Faker.PhoneFaker.InternationalPhone() ) },
        {"$lorem.sentence", new Func<string>(()=> Faker.Lorem.Sentence() ) },
        {"$lorem.paragraph", new Func<string>(()=> Faker.Lorem.Sentence() ) },
        {"$address.country", new Func<string>(()=> Faker.Address.Country() ) },
        {"$address.city", new Func<string>(()=> Faker.Address.City() ) },
        {"$address.zipcode", new Func<string>(()=> Faker.Address.ZipCode() ) },
        {"$address.streetName", new Func<string>(()=> Faker.Address.StreetName() ) },
        {"$address.streetAddress", new Func<string>(()=> Faker.Address.StreetAddress() ) },
        {"$date.birth", new Func<object>(()=> Faker.DateTimeFaker.BirthDay() ) },
        {"$random.number", new Func<object>(()=> Faker.RandomNumber.Next() ) },
        {"$random.date", new Func<object>(()=> Faker.DateTimeFaker.DateTimeBetweenYears(0, 1)) },
        {"$random.bool", new Func<object>(()=> Faker.BooleanFaker.Boolean() ) },
        {"$random.alpha", new Func<object>(()=> Faker.StringFaker.Alpha(5) ) },
        {"$random.alphaNumeric", new Func<object>(()=> Faker.StringFaker.AlphaNumeric(5) ) },
        {"$random.numeric", new Func<object>(()=> Faker.StringFaker.Numeric(8) ) },
        {"$null", new Func<object>(() => {
            return null;
        }) }
    };
    }
}


//{
//          "$comment": {
//            "count": 10
//          }


[JsonDictionary]
public class MockArrayExpression : Dictionary<string, MockArrayExpression>
{

}

public class MockArrayExpressionMeta
{
    [JsonProperty("count")]
    public int Count { get; set; }
}