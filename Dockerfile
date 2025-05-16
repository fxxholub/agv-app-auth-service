# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION={$BUILD_CONFIGURATION:-Release}
WORKDIR /src
COPY ["AgvAppAuthService/AgvAppAuthService/AgvAppAuthService.csproj", "AgvAppAuthService/"]
RUN dotnet restore "AgvAppAuthService/AgvAppAuthService.csproj"
COPY ./AgvAppAuthService .
WORKDIR "/src/AgvAppAuthService"
RUN mkdir -p /app/data && chown -R $USER:$USER /app/data
RUN dotnet build "AgvAppAuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION={$BUILD_CONFIGURATION:-Release}
RUN dotnet publish "AgvAppAuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Startup stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS startup
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AgvAppAuthService.dll"]