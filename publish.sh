#!/bin/bash

dotnet publish src/dotnet-mock-server/dotnet-mock-server.csproj \
	--configuration Release \
	--output build \
	-p:PublishReadyToRun=false \
	-p:PublishSingleFile=true \
	-p:PublishTrimmed=true \
	--self-contained true \
	--runtime win-x64 && \
	rm build/*.pdb && 
	mv build/dotnet-mock-server.exe build/nmock.exe	