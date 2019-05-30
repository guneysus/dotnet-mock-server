$VERSION = "1.0.1"

dotnet pack src/dotnet-mock-server

echo "nuget push src/dotnet-mock-server/dotnet-mock-server.$VERSION.nupkg $env:MYGET_SECRET -Source $env:MYGET_SOURCE"