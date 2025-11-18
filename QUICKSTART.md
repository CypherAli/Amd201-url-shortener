# ⚡ Quick Start Guide

Get AMD201 URL Shortener running in 5 minutes!

## 🚀 Option 1: Docker Compose (Fastest)

```bash
# Clone
git clone https://github.com/yourusername/amd201.git
cd amd201

# Update docker-compose.yml with your Supabase JWT secret
# (Line 27: - Supabase__JwtSecret=your-secret-here)

# Run
docker-compose up --build

# Access:
# - API: http://localhost:5000
# - Swagger: http://localhost:5000/swagger
```

## 🔧 Option 2: Local Development

```bash
# Prerequisites: .NET 9.0 SDK

# Clone and build
git clone https://github.com/yourusername/amd201.git
cd amd201
dotnet build

# Configure
# Edit src/AMD201.API/appsettings.Development.json
# Set your Supabase JWT secret

# Run
cd src/AMD201.API
dotnet run

# Access: http://localhost:5000
```

## 🌐 Option 3: Use Frontend

1. Open `frontend/index.html` in a text editor
2. Update lines 183-185:
   ```javascript
   const SUPABASE_URL = 'your-supabase-url';
   const SUPABASE_ANON_KEY = 'your-supabase-anon-key';
   const API_BASE_URL = 'http://localhost:5000';
   ```
3. Open `frontend/index.html` in your browser

## 📋 Get Supabase Credentials

1. Go to https://supabase.com
2. Create account (free)
3. New Project
4. Get credentials from Project Settings → API:
   - Project URL
   - Anon Key
   - JWT Secret

## ✅ Test It

```bash
# Test URL shortening
curl -X POST http://localhost:5000/api/url/shorten \
  -H "Content-Type: application/json" \
  -d '{"originalUrl": "https://www.google.com"}'

# Test redirect
curl -I http://localhost:5000/{shortCode}
```

## 🐛 Common Issues

**Port 5000 already in use?**
```bash
# Kill process on port 5000 (Windows)
netstat -ano | findstr :5000
taskkill /PID <pid> /F

# Or change port in Program.cs
```

**Database error?**
```bash
# Delete and recreate
del src\AMD201.API\urlshortener.db*
dotnet run
```

## 📚 Full Documentation

- [README.md](README.md) - Complete documentation
- [SETUP_GUIDE.md](SETUP_GUIDE.md) - Detailed setup
- [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md) - Project overview

## 🎯 Next Steps

1. ✅ Get it running locally
2. ✅ Test with Postman/Swagger
3. ✅ Create Supabase account
4. ✅ Configure authentication
5. ✅ Deploy to Render
6. ✅ Set up CI/CD

**Need help?** Check [SETUP_GUIDE.md](SETUP_GUIDE.md) for detailed instructions.
