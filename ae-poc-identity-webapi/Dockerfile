# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
# Health Check Port
EXPOSE 9008

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ae-poc-identity-webapi/ae-poc-identity-webapi.csproj", "ae-poc-identity-webapi/"]
COPY ["ae-poc-identity-services/ae-poc-identity-services.csproj", "ae-poc-identity-services/"]
COPY ["ae-poc-identity-storage/ae-poc-identity-storage.csproj", "ae-poc-identity-storage/"]
COPY ["ae-poc-identity-core/ae-poc-identity-core.csproj", "ae-poc-identity-core/"]
RUN dotnet restore "./ae-poc-identity-webapi/ae-poc-identity-webapi.csproj"
COPY . .
WORKDIR "/src/ae-poc-identity-webapi"
RUN dotnet build "./ae-poc-identity-webapi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ae-poc-identity-webapi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .

# Health Check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:9008/health/live || exit 1

ENTRYPOINT ["dotnet", "ae-poc-identity-webapi.dll"]