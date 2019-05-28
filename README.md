# mock-server



## installing

```
dotnet tool install -v n --version 1.0.0 --no-cache --global --add-source https://www.myget.org/F/guneysu-sandbox/api/v3/index.json dotnet-mock-server
```

## Local Installing
```
dotnet pack src/dotnet-mock-server
dotnet tool install -v n --global --add-source src/dotnet-mock-server/nupkg dotnet-mock-server
```

## uninstall
```
dotnet tool uninstall -g dotnet-mock-server
```
