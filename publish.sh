#!/bin/bash

dotnet publish src/dotnet-mock-server/dotnet-mock-server.csproj \
	--configuration Release \
	--output build \
	-p:PublishReadyToRun=false \
	-p:PublishSingleFile=true \
	-p:PublishTrimmed=false \
	--self-contained true \
	--runtime win-x64