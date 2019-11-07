# Google Drive .net core - Web API

## Description
This project is an implementation of the [Google Drive .net core class library](https://github.com/LouisChering/GoogleDrive.Core.DotnetCore) exposed as a web API.

## Pre-requisites
To communicate with the Google Drive API you need to have setup a project within the Google Cloud Console and have provisioned some endpoints.  
[See this page for instructions](https://developers.google.com/drive/api/v3/quickstart/nodejs)

You will need the credentials.json file generated as a result of this process. This is the final part of step 1.

## Build

### Docker
The easiest way to build and run this project is to use the included dockerfile.
```
docker build -t drive-api .
```

The dockerfile is using Microsoft's recommedation of a multi-step build container.  The first phase is carried out using an image from the .Net Core SDK.  The second phase image is built using only the runtime. This causes the output image size is smaller. 

[See this for more information](https://docs.docker.com/engine/examples/dotnetcore/)

### .Net Core
To build the code using your current platform as a target use:
```
dotnet clean; dotnet publish -c Release -o /app
```
To target another runtime use:
```
dotnet publish -c Release -o /app --runtime ubuntu.18.04-x64=
```

## Run
### Docker
Run built image locally:
```
docker run -d -p 8080:8080 --name mydriveapi drive-api
```
*Be aware that you will lose any credentials generated in this state*
Run built image locally with volume for persistent credentials:
```
docker run -d -p 8080:8080 -v credentials.json:/app/credentials.json -v token.json:/app/token.json --name mydriveapi drive-api
```
### .Net Core
Ensure you have copied *credentials.json* to the same folder as the built .dll.  Then run:
```
dotnet aspnetcore-2-webapi.dll
```
