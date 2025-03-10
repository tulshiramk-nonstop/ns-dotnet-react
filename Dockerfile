FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["SimpleApi/SimpleApi.csproj", "./"]
RUN dotnet restore

# Copy everything else and build
COPY SimpleApi/. .
RUN dotnet publish -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "SimpleApi.dll"]