# ---------- Base ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Generate self-signed cert
RUN apt-get update && apt-get install -y openssl && \
    mkdir -p /https && \
    openssl req -x509 -newkey rsa:2048 -nodes \
        -keyout /https/key.pem \
        -out /https/cert.pem \
        -days 3650 \
        -subj "/CN=localhost" && \
    openssl pkcs12 -export \
        -out /https/aspnetapp.pfx \
        -inkey /https/key.pem \
        -in /https/cert.pem \
        -passout pass:H1707DireWolf!?

EXPOSE 8080
EXPOSE 443

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

ENV ASPNETCORE_URLS=https://+:443;http://+:8080
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=H1707DireWolf!?

ENTRYPOINT ["dotnet", "Authentication.API.dll"]
