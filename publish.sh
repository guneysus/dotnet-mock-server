#!/bin/bash

dotnet publish src/dotnet-mock-server/dotnet-mock-server.csproj \
	--configuration Release \
	--output build \
	--no-build \
	-p:PublishReadyToRun=false \
	-p:PublishSingleFile=true \
	-p:PublishTrimmed=true \
	--self-contained true \
	--runtime win-x64
	
	
dotnet publish src/dotnet-mock-server/dotnet-mock-server.csproj -c Release -o build --no-build -p:PublishReadyToRun=false -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true -r win-x64	