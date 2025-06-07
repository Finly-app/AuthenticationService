# ---------- Base ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080


# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Authentication.API/Authentication.API.csproj"
RUN dotnet build "Authentication.API/Authentication.API.csproj" -c Release -o /app/build

# ---------- Publish ----------
FROM build AS publish
RUN dotnet publish "Authentication.API/Authentication.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- Final ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.API.dll"]
