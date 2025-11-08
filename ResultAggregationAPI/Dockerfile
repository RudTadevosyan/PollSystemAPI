# === BUILD ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# COPY ENTIRE SOLUTION
COPY . .

# RESTORE
RUN dotnet restore "ResultAggregationAPI/ResultAggregation.API/ResultAggregation.API.csproj"

# PUBLISH
WORKDIR /src/ResultAggregationAPI/ResultAggregation.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# === RUNTIME ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "ResultAggregation.API.dll"]
