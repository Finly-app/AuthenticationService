# ---------- Base ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 5005  # Expose debugger port


# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Authentication.API/Authentication.API.csproj"
RUN dotnet build "Authentication.API/Authentication.API.csproj" -c Debug -o /app/build


# ---------- Publish ----------
FROM build AS publish
RUN dotnet publish "Authentication.API/Authentication.API.csproj" -c Debug -o /app/publish /p:UseAppHost=false


# ---------- Final ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Install vsdbg
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

COPY --from=publish /app/publish .

# Set default command to dotnet (you can override this to start vsdbg manually)
ENTRYPOINT ["dotnet", "Authentication.API.dll"]
