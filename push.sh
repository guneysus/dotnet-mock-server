#!/bin/bash

set -ex

VERSION=1.0.1

dotnet pack src/dotnet-mock-server

echo "nuget push src/dotnet-mock-server/dotnet-mock-server.$VERSION.nupkg $MYGET_SECRET -Source $MYGET_SOURCE"