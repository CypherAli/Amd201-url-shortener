# AMD201 URL Shortener

A modern URL shortening service built with **ASP.NET Core 9.0** and containerized with **Docker**.

## 🚀 Quick Start

### Pull and Run Docker Image

```bash
docker pull doquanganh/url-shortener:latest
docker run -d -p 8080:8080 doquanganh/url-shortener:latest
```

Then access: **http://localhost:8080**

### Run Locally

```bash
cd src/AMD201.API
dotnet run
```

Access: **http://localhost:8080**

## ✨ Features

- 🔗 **URL Shortening** - Convert long URLs to short codes
- 📊 **Analytics** - Track click statistics and user analytics
- 🔐 **Authentication** - Supabase OAuth integration
- 📱 **QR Code** - Auto-generate QR codes for shortened URLs
- 📚 **API Documentation** - Swagger/OpenAPI support
- 🐳 **Docker Ready** - Multi-stage Docker build (126.4 MB)
- ✅ **Well-tested** - Unit tests included

## 📋 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Health check / Frontend |
| GET | `/swagger/ui` | API Documentation |
| POST | `/api/urls/shorten` | Create shortened URL |
| GET | `/r/{shortCode}` | Redirect to original URL |

## 🏗️ Architecture

```
AMD201.API (Web API)
├── Controllers (URL & Redirect handlers)
├── Middleware (Supabase Authentication)
└── wwwroot (Frontend)

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
- **Framework**: ASP.NET Core
- **Database**: SQLite (Entity Framework Core)
- **Authentication**: Supabase
- **Container**: Docker (Linux/amd64)
- **Documentation**: Swagger/OpenAPI

## 📦 Docker Image Details

- **Repository**: `doquanganh/url-shortener`
- **Latest Tag**: `sha256:245ead9a64a6`
- **Size**: 126.4 MB
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
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
```

### Environment Variables

- `ASPNETCORE_URLS` - Server URL (default: `http://+:8080`)
- `ASPNETCORE_ENVIRONMENT` - Environment (default: `Production`)
- `SUPABASE_URL` - Supabase project URL
- `SUPABASE_KEY` - Supabase API key

## 📊 Performance

- Multi-stage Docker build for minimal image size
- Database migrations on startup
- Caching support for click statistics
- Optimized for cloud deployment

## 📝 License

MIT License - See LICENSE file

## 👤 Author

- **anhdqgch220999-dotcom** - Backend Developer
- **CypherAli** - Project Owner

## 🔗 Repository

- **GitHub**: https://github.com/CypherAli/Amd201-url-shortener
- **Docker Hub**: https://hub.docker.com/r/doquanganh/url-shortener

---

**Last Updated**: November 24, 2025  
**Docker Image Size**: 126.4 MB  
**Active**: ✅ Production Ready
