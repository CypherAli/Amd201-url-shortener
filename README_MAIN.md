# AMD201 URL Shortener

A modern, production-ready URL shortening service built with **ASP.NET Core 9.0** and containerized with **Docker**.

## 🚀 Quick Start

### Docker (Recommended)

```bash
docker pull doquanganh/url-shortener:latest
docker run -d -p 8080:8080 doquanganh/url-shortener:latest
```

Access: **http://localhost:8080**

### Local Development

```bash
cd src/AMD201.API
dotnet run
```

Access: **http://localhost:8080**

## ✨ Features

- 🔗 **URL Shortening** - Convert long URLs to short codes
- 📊 **Analytics** - Track click statistics and user behavior
- 🔐 **Authentication** - Supabase OAuth integration
- 📱 **QR Codes** - Auto-generate QR codes for shortened URLs
- 📚 **API Documentation** - Swagger/OpenAPI support
- 🐳 **Production Docker Image** - Multi-stage optimized (126.4 MB)
- ✅ **Well-tested** - Unit tests included and passing
- ⚡ **High Performance** - SQLite + EF Core with caching

## 📋 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Health check / Frontend |
| GET | `/swagger/ui` | API Documentation |
| POST | `/api/urls/shorten` | Create shortened URL |
| GET | `/r/{shortCode}` | Redirect to original URL |
| GET | `/api/urls/{shortCode}/stats` | Get click statistics |

## 🏗️ Architecture

```
AMD201.API (Web API)
├── Controllers (URL & Redirect handlers)
├── Middleware (Supabase Authentication)
└── wwwroot (Frontend assets)

AMD201.Core (Domain Models)
├── Entities (ShortenedUrl, ClickStatistic)
├── Interfaces (Repository, Service contracts)
└── DTOs (Data Transfer Objects)

AMD201.Infrastructure (Data Access)
├── Data (DbContext & Migrations)
├── Repositories (URL data access)
└── Services (Business logic)
```

## 🛠️ Technology Stack

- **Runtime**: .NET 9.0
- **Framework**: ASP.NET Core Web API
- **Database**: SQLite with Entity Framework Core
- **Authentication**: Supabase
- **Container**: Docker (Linux amd64)
- **Documentation**: Swagger/OpenAPI 3.0
- **Testing**: xUnit

## 📦 Docker Image Details

- **Repository**: `doquanganh/url-shortener`
- **Latest Tag**: `latest` (sha256:245ead9a64a6)
- **Compressed Size**: 126.4 MB
- **Base Image**: `mcr.microsoft.com/dotnet/aspnet:9.0`
- **OS/Architecture**: Linux amd64
- **Health Check**: HTTP GET on port 8080

## 🚀 Deployment

### Docker Compose

```yaml
version: '3.8'
services:
  url-shortener:
    image: doquanganh/url-shortener:latest
    container_name: amd201-url-shortener
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
    restart: unless-stopped
```

Run:
```bash
docker-compose up -d
```

### Kubernetes (Helm)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: url-shortener
spec:
  replicas: 2
  selector:
    matchLabels:
      app: url-shortener
  template:
    metadata:
      labels:
        app: url-shortener
    spec:
      containers:
      - name: url-shortener
        image: doquanganh/url-shortener:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: Production
```

## 🔧 Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `ASPNETCORE_URLS` | `http://+:8080` | Server binding URL |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Environment mode |
| `SUPABASE_URL` | - | Supabase project URL |
| `SUPABASE_KEY` | - | Supabase API key |

## 📊 Performance Metrics

- **Image Build Time**: ~20 seconds (with cache)
- **Startup Time**: ~2 seconds
- **Database Migrations**: Automatic on startup
- **Response Time**: < 100ms (median)
- **Memory Usage**: ~80-150 MB

## 🧪 Testing

```bash
# Run unit tests
dotnet test tests/AMD201.Tests/AMD201.Tests.csproj

# Build Docker image and run tests
docker build -t url-shortener:test .
```

## 📝 Project Structure

```
d:\Amd201-url-shortener/
├── src/
│   ├── AMD201.API/           # Web API (ASP.NET Core)
│   ├── AMD201.Core/          # Domain models & interfaces
│   └── AMD201.Infrastructure/ # Data access & services
├── tests/
│   └── AMD201.Tests/         # Unit tests
├── frontend/                 # Vue.js frontend (optional)
├── Dockerfile               # Multi-stage Docker build
├── docker-compose.yml       # Docker Compose configuration
└── README.md               # This file
```

## 🔒 Security

- ✅ Non-root container user (appuser)
- ✅ Input validation on all endpoints
- ✅ Supabase authentication for protected endpoints
- ✅ HTTPS ready (configure in production)
- ✅ No hardcoded secrets

## 📚 Documentation

- **API Docs**: http://localhost:8080/swagger/ui
- **GitHub**: https://github.com/CypherAli/Amd201-url-shortener
- **Docker Hub**: https://hub.docker.com/r/doquanganh/url-shortener

## 👥 Contributors

- **anhdqgch220999-dotcom** - Backend Developer
- **CypherAli** - Project Owner

## 📄 License

MIT License - See [LICENSE](LICENSE) file for details

## 🤝 Support

- GitHub Issues: https://github.com/CypherAli/Amd201-url-shortener/issues
- Docker Hub: https://hub.docker.com/r/doquanganh/url-shortener

---

**Last Updated**: November 24, 2025  
**Status**: ✅ Production Ready  
**Docker Image Version**: 126.4 MB
