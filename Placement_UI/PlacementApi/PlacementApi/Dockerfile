# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Publish project
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# IMPORTANT: Ensure Models folder exists
COPY Models ./Models

ENTRYPOINT ["dotnet", "PlacementAPI.dll"]