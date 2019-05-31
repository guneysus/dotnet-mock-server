# mock-server

declarative mock server with fake data generation capabilities for .NET Core 2.1

this project started after meeting https://github.com/gencebay and learning his cool project https://github.com/gencebay/httplive

## requirements

- .NET Core 2.1 SDK

## Installation

```
dotnet tool install -v n --no-cache --global --add-source https://www.myget.org/F/guneysu/api/v3/index.json dotnet-mock-server
```

## Arguments

```
dotnet-mock-server:
  Mock Server

Usage:
  dotnet-mock-server [options]

Options:
  --generate-config <generate-config>    An option whose argument is parsed as an int
  --config <config>                      An option whose argument is parsed as an int
  --version                              Display version information
```

## Running

```
dotnet mock-server      # recommended
dotnet-mock-server.exe  # for windows
dotnet-mock-server      # linux
```

## Local Installation

```
dotnet pack src/dotnet-mock-server
dotnet tool install -v n --global --add-source src/dotnet-mock-server/nupkg dotnet-mock-server
```

## Uninstallation

```
dotnet tool uninstall -g dotnet-mock-server
```

## Default Behaviour

The default behaviour of mock-server is creating a default configuration file named `mockServer.json` in current working directory.

But lets start with simple.

Config file schema is below:

```json
{
  "resources": {
    "<URL>": {
      "<METHOD>": {
        "content": "array|object|any primitive type (required)",
        "contentType": "<Content Type Header> (optional)",
        "statusCode": 200, // optional, 200-599 integer value
        "dynamic": true, // optional, default false
        "headers": {
          // optional
          "<KEY_1>": "<VALUE_1>",
          "<KEY_2>": "<VALUE_2>"
        }
      }
    }
  }
}
```

## Usage

**Hello world**

```json
{
  "resources": {
    "/": {
      "get": {
        "content": "Hello world."
      }
    }
  }
}
```

```http
HTTP/1.1 200 OK
Content-Length: 12
Date: Fri, 31 May 2019 08:25:09 GMT
Server: Kestrel

Hello world.
```

Lets respond with a HTML content.

```json
{
  "resources": {
    "/": {
      "get": {
      "content": "<strong>Hello <i>world</i>",
      "contentType": "text/html"
      }
    }
  }
}
```


Will render in browser in bold and italic.

```http
HTTP/1.1 200 OK
Content-Length: 26
Content-Type: text/html
Date: Fri, 31 May 2019 08:32:55 GMT
Server: Kestrel

<strong>Hello <i>world</i>
```

**Lets return and valid XML content. But we should escape double quotes, forward slashes:**

Like: 
- `key="foo"` to `key=\"foo\"`
- `<from>Jani</from>` to `<from>Jani<\/from>`

```json
{
  "resources": {
    "/xml": {
      "get": {
        "content": "<?xml version=\"1.0\" encoding=\"UTF-8\"?><note>  <to>Tove<\/to>  <from>Jani<\/from>  <heading>Reminder<\/heading>  <body>Don't forget me this weekend!<\/body><\/note>",
        "contentType": "application/xml"
      }
    }
  }
}

```    

```http
> http :5003/xml
HTTP/1.1 200 OK
Content-Length: 158
Content-Type: application/xml
Date: Fri, 31 May 2019 08:41:36 GMT
Server: Kestrel

<?xml version="1.0" encoding="UTF-8"?><note>  <to>Tove</to>  <from>Jani</from>  <heading>Reminder</heading>  <body>Don't forget me this weekend!</body></note>

```

**Adding headers to response:**

```json
{
  "resources": {
    "/": {
      "get": {
        "content": "Hello world"
      }
    },

    "/headers": {
      "get": {
        "content": "Hello World",
        "headers": {
          "X-Powered-By": "MockServer.NET"
        }
      }
    }
  }
}
```

```http
> http :5003/headers
HTTP/1.1 200 OK
Content-Length: 11
Date: Fri, 31 May 2019 08:44:55 GMT
Server: Kestrel
X-Powered-By: MockServer.NET

Hello World
```

You can set cookie, redirect pages, setting client side cache, etc with adding headers.

**Lets redirect a url to example.com.**

We will add a Location haeader but in order to work redirects we must also set status code to 3XX (300-308). We use 301. See: https://en.wikipedia.org/wiki/List_of_HTTP_status_codes#3xx_Redirection


```json
{
  "resources": {
    "/redirect": {
      "get": {
        "headers": {
          "Location": "http://example.com"
        },
        "statusCode": "301"
      }
    }
  }
}
```

```http
> http :5003/redirect --follow
HTTP/1.1 200 OK
Accept-Ranges: bytes
Cache-Control: max-age=604800
Content-Encoding: gzip
Content-Length: 606
Content-Type: text/html; charset=UTF-8
Date: Fri, 31 May 2019 10:29:58 GMT
Etag: "1541025663"
Expires: Fri, 07 Jun 2019 10:29:58 GMT
Last-Modified: Fri, 09 Aug 2013 23:54:35 GMT
Server: ECS (dcb/7EA3)
Vary: Accept-Encoding
X-Cache: HIT

<!doctype html>
<html>
<head>
    <title>Example Domain</title>
...
```    

### Returning JSON and generation fake data on the fly:

According to me the coolest part of this side project is generating (or compiling) mock data expressions defined in JSON properties.


```
{
  "resources": {
    "/api/comment/{id:int}": {
      "get": {
        "contentType": "application/json",
        "dynamic": true,
        "content": {
          "_id": "fn:guid",
          "user": {
            "name": "fn:fullname"
          },
          "date": "fn:random.date"
        }
      }
    }
  }
}
```

```http
> http :5003/api/comment/100
HTTP/1.1 200 OK
Content-Length: 125
Content-Type: application/json
Date: Fri, 31 May 2019 11:27:15 GMT
Server: Kestrel

{
    "_id": "5187ac27-d326-4ce8-a4c5-baf7be078208",
    "date": "2019-06-30T10:55:01.66917+03:00",
    "user": {
        "name": "Miss Vaughn Kautzer"
    }
}
```

- `"/api/comment/{id:int}"` line will catch all URLS ending with integer values.
- `"fn:guid"`   generates random guid on each request


For now, `dynamic=True` should be set. But this be removed probably in future.
And also I am planning to change from `fn:` prefixes to `$`.

Other dynamic values for mock data generation is below:

- fn:guid
- fn:name
- fn:fullname
- fn:email
- fn:userName
- fn:phoneNumber
- fn:lorem.sentence
- fn:lorem.paragraph
- fn:address.country
- fn:address.city
- fn:address.zipcode
- fn:address.streetName 
- fn:address.streetAddress
- fn:date.birth
- fn:random.number
- fn:random.date
- fn:random.bool
- fn:random.alpha
- fn:random.alphaNumeric
- fn:random.numeric
- fn:null


## Generate from predefined templates: [WIP]

I am working on predefined section to generate array of data. I am reserved the `templates` section for this:

```json
  "templates": {
    "$comment": {
      "_id": "fn:guid",
      "user": {
        "name": "fn:fullname"
      },
      "date": "fn:random.date"
    }
  },
```

And will be used in resources section like this:

```json
{
  "templates": {
    "$comment": {
      "_id": "fn:guid",
      "user": {
        "name": "fn:fullname"
      },
      "date": "fn:random.date"
    }
  },
  "resources": {
        "/api/comment": {
      "get": {
        "dynamic":  true,
        "content": {
          "$comment": {
            "count": 10
          }
        }
      }
    }
  }
}
```
or "content": "$comment * N"` which N is a positive number.

---
## Related projects
- There is also many projects generates mock APIs from Swagger or RAML files.

---

## TODOS

- Resource template definitions
- Echo route, query and form parameters
- Return content from files.
- Lots of refactoring
