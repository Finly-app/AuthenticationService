# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ⬅️ Copy everything from the build context (which is one level up)
COPY ../ . 

# Restore and build the correct project
RUN dotnet restore "Authentication.API/Authentication.API.csproj"
RUN dotnet build "Authentication.API/Authentication.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Authentication.API/Authentication.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

RUN echo "----- FILES IN /app/publish -----" && ls -R /app/publish

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.API.dll"]
