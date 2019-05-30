using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Recursive Converter
/// https://stackoverflow.com/a/6417753/1766716
/// </summary>
public class DynamicJsonConverter : CustomCreationConverter<IDictionary<string, object>>
{
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
        Func<object> fn;

        if (Fn.TryGetValue(obj.ToString(), out fn))
        {
            return fn();
        }

        return obj;
    }

    public Dictionary<string, Func<object>> Fn { get; set; } = new Dictionary<string, Func<object>>()
    {
        {"fn:guid", new Func<string>(()=> Guid.NewGuid().ToString() ) },
        {"fn:name", new Func<string>(()=> Faker.Internet.UserName() ) },
        {"fn:fullname", new Func<string>(()=> Faker.Name.FullName() ) },
        {"fn:email", new Func<string>(()=> Faker.Internet.Email()) },
        {"fn:userName", new Func<string>(()=> Faker.Internet.UserName() ) },
        {"fn:phoneNumber", new Func<string>(()=> Faker.PhoneFaker.InternationalPhone() ) },
        {"fn:lorem.sentence", new Func<string>(()=> Faker.Lorem.Sentence() ) },
        {"fn:lorem.paragraph", new Func<string>(()=> Faker.Lorem.Sentence() ) },
        {"fn:address.country", new Func<string>(()=> Faker.Address.Country() ) },
        {"fn:address.city", new Func<string>(()=> Faker.Address.City() ) },
        {"fn:address.zipcode", new Func<string>(()=> Faker.Address.ZipCode() ) },
        {"fn:address.streetName", new Func<string>(()=> Faker.Address.StreetName() ) },
        {"fn:address.streetAddress", new Func<string>(()=> Faker.Address.StreetAddress() ) },
        {"fn:date.birth", new Func<object>(()=> Faker.DateTimeFaker.BirthDay() ) },
        {"fn:random.number", new Func<object>(()=> Faker.RandomNumber.Next() ) },
        {"fn:random.date", new Func<object>(()=> Faker.DateTimeFaker.DateTimeBetweenYears(0, 1)) },
        {"fn:random.bool", new Func<object>(()=> Faker.BooleanFaker.Boolean() ) },
        {"fn:random.alpha", new Func<object>(()=> Faker.StringFaker.Alpha(5) ) },
        {"fn:random.alphaNumeric", new Func<object>(()=> Faker.StringFaker.AlphaNumeric(5) ) },
        {"fn:random.numeric", new Func<object>(()=> Faker.StringFaker.Numeric(8) ) },
        {"fn:random.pattern", new Func<object>(()=> Faker.StringFaker.Randomize("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")) },

        {"fn:null", new Func<object>(() => {
            return null;
        }) }
    };
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