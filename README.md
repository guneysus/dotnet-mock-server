# mock-server

declarative mock server with fake data generation capabilities.

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
