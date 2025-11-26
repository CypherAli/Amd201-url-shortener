# Multi-stage Dockerfile for AMD201 URL Shortener
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["AMD201.sln", "./"]
COPY ["src/AMD201.API/AMD201.API.csproj", "src/.AMD201.API/"]
COPY ["src/AMD201.Core/AMD201.Core.csproj", "src/AMD201.Core/"]
COPY ["src/AMD201.Infrastructure/AMD201.Infrastructure.csproj", "src/AMD201.Infrastructure/"]
COPY ["tests/AMD201.Tests/AMD201.Tests.csproj", "tests/AMD201.Tests/"]

# Restore dependencies
RUN dotnet restore "AMD201.sln"

# Copy all source files
COPY . .

# Run tests
WORKDIR "/src/tests/AMD201.Tests"
RUN dotnet test --no-restore --verbosity normal

# Build and publish
WORKDIR "/src/src/AMD201.API"
RUN dotnet publish "AMD201.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user
RUN useradd -m -u 1000 appuser && chown -R appuser /app
USER appuser

# Copy published app from build stage
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "AMD201.API.dll"]
