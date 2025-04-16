# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ✅ Correcting the COPY command
COPY ["Authentication.API/Authentication.API.csproj", "./Authentication.API/"]

# ✅ Correcting the dotnet restore command
RUN dotnet restore "Authentication.API/Authentication.API.csproj"

# Copy everything into the container
COPY . .

WORKDIR "/src/Authentication.API"

# Build the project
RUN dotnet build "Authentication.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Authentication.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Release
ENTRYPOINT ["dotnet", "Authentication.API.dll"]

