FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env

# Copy csproj and restore as distinct layers
COPY . /source/
WORKDIR /source

RUN dotnet restore
RUN mkdir /app/
RUN dotnet publish -c Release -o /app
RUN rm -r /app/submodules/

# RUN cp -r token.json /app
# RUN cp credentials.json /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
COPY --from=build-env /app /app
WORKDIR /app
RUN dir /app
ENTRYPOINT ["dotnet", "aspnetcore-2-webapi.dll"]