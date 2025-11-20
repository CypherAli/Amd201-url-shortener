# AMD201 URL Shortener - Docker Image

URL Shortener application built with ASP.NET Core 9.0

## Quick Start

### Pull the image
```bash
docker pull doquanganh/url-shortener:latest
```

### Run the container
```bash
docker run -p 8080:8080 doquanganh/url-shortener:latest
```

### With port mapping
```bash
docker run -p 3000:8080 doquanganh/url-shortener:latest
```

## Environment Variables

- `ASPNETCORE_URLS`: Default is `http://+:8080`
- `ASPNETCORE_ENVIRONMENT`: Default is `Production`

## Features

- ✅ Shorten long URLs
- ✅ QR Code generation
- ✅ Click statistics tracking
- ✅ User authentication via Supabase
- ✅ Swagger/OpenAPI documentation

## API Endpoints

- `GET /` - Health check / Frontend
- `GET /swagger/ui` - Swagger UI
- `POST /api/urls/shorten` - Create shortened URL
- `GET /r/{shortCode}` - Redirect to original URL

## Requirements

- Docker Desktop
- Port 8080 available

## Build Locally

```bash
docker build -t url-shortener:latest .
```

## License

MIT
