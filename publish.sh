#!/bin/bash

set -x

dotnet publish src/ \
	--configuration Release \
	--output build \
	--no-build \
	-p:PublishReadyToRun=false \
	-p:PublishSingleFile=true \
	-p:PublishTrimmed=true \
	--self-contained true \
	--runtime win-x64

# rm build/*.pdb && 
mv build/dotnet-mock-server.exe build/nmock.exe	