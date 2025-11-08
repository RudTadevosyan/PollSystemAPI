# === BUILD ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# COPY EVERYTHING
COPY . .

# RESTORE
RUN dotnet restore "PollManagementAPI/PollManagement.API/PollManagement.API.csproj"

# INSTALL EF CORE CLI GLOBALLY
RUN dotnet tool install --global dotnet-ef --version 9.0.10
ENV PATH="$PATH:/root/.dotnet/tools"

# PUBLISH
WORKDIR /src/PollManagementAPI/PollManagement.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# === RUNTIME ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# COPY EF TOOL FROM SDK IMAGE
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="$PATH:/root/.dotnet/tools"

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "PollManagement.API.dll"]
