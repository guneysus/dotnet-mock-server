dotnet publish .\dotnet-mock-server\dotnet-mock-server.csproj `
	--configuration Release `
	--output build `
	-p:PublishReadyToRun=true `
	-p:PublishSingleFile=true `
	-p:PublishTrimmed=true `
	--self-contained true `
	--runtime win-x64 
	
mv build\dotnet-mock-server.exe build\nmock.exe