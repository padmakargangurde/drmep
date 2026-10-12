# =========================
# Build Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and project files
COPY MEP-APIs.sln ./
COPY MEP-APIs/MEP-APIs.csproj MEP-APIs/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/

# Restore dependencies
RUN dotnet restore MEP-APIs.sln

# Copy source code
COPY . .

# Build
RUN dotnet build MEP-APIs/MEP-APIs.csproj \
    -c Release \
    --no-restore

# Publish
RUN dotnet publish MEP-APIs/MEP-APIs.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


# =========================
# Runtime Stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# ASP.NET Core listens on port 8080
EXPOSE 8080

# Start API
ENTRYPOINT ["dotnet", "MEP-APIs.dll"]