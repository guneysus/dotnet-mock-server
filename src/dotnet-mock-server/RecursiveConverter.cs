using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
/// <summary>
/// Recursive Converter
/// https://stackoverflow.com/a/6417753/1766716
/// </summary>
internal class RecursiveConverter : CustomCreationConverter<IDictionary<string, object>>
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
        if (reader.TokenType == JsonToken.StartObject
            || reader.TokenType == JsonToken.Null)
        {
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        // if the next token is not an object
        // then fall back on standard deserializer (strings, numbers etc.)
        object obj = serializer.Deserialize(reader);
        object str = Parse(obj);
        return str;
    }

    protected object Parse(object obj)
    {
        Func<string> fn;

        if (Fn.TryGetValue(obj.ToString(), out fn))
        {
            return fn();
        }

        return obj;
    }

    public Dictionary<string, Func<string>> Fn { get; set; } = new Dictionary<string, Func<string>>()
    {
        {"fn:guid", new Func<string>(()=> Guid.NewGuid().ToString() ) },
        {"fn:fullname", new Func<string>(()=> Faker.Name.FullName() ) },
        {"fn:email", new Func<string>(()=> Faker.Internet.Email() ) },
        {"fn:username", new Func<string>(()=> Faker.Internet.UserName() ) },
        {"fn:phonenumber", new Func<string>(()=> Faker.Phone.Number() ) },
    };
}
