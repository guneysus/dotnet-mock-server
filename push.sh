#!/bin/bash

set -ex

VERSION=1.0.1

dotnet pack src/dotnet-mock-server

nuget push src/dotnet-mock-server/nupkg/dotnet-mock-server.$VERSION.nupkg $MYGET_SECRET -Source $MYGET_SOURCE