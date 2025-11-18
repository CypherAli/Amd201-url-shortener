# 🚀 HƯỚNG DẪN CHẠY PROJECT - URL SHORTENER

## 📋 Yêu cầu hệ thống

- **.NET SDK 9.0** hoặc cao hơn
- **Visual Studio 2022** hoặc **VS Code**
- **PostgreSQL** (hoặc dùng Supabase)
- **Git** (để clone/push code)

---

## 🔧 Cấu hình Database

### Option 1: Sử dụng Supabase (Đề xuất - Miễn phí)

1. Đã có sẵn trong `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.mpuomfxrhhdlgidjujtt;Password=Haibinz2005"
}
```

2. Supabase Auth đã config sẵn trong `frontend/index.html`

### Option 2: Dùng PostgreSQL local

1. Cài PostgreSQL
2. Tạo database mới
3. Sửa connection string trong `appsettings.json`

---

## 🏃 CÁCH 1: Chạy bằng Visual Studio 2022

### Bước 1: Mở Solution
1. Double-click file `AMD201.sln`
2. Hoặc: Mở Visual Studio → **File** → **Open** → **Project/Solution** → Chọn `AMD201.sln`

### Bước 2: Restore NuGet Packages
- Visual Studio sẽ tự động restore packages
- Hoặc: **Tools** → **NuGet Package Manager** → **Restore NuGet Packages**

### Bước 3: Apply Database Migrations
1. Mở **Package Manager Console**: **Tools** → **NuGet Package Manager** → **Package Manager Console**
2. Chọn **Default project**: `AMD201.Infrastructure`
3. Chạy lệnh:
```powershell
Update-Database
```

### Bước 4: Set Startup Project
1. Right-click vào project `AMD201.API` trong Solution Explorer
2. Chọn **Set as Startup Project**

### Bước 5: Run Project
- Nhấn **F5** hoặc click nút **Run** (Start Debugging)
- Hoặc **Ctrl + F5** (Start Without Debugging)

### Bước 6: Truy cập ứng dụng
- Browser sẽ tự động mở: `https://localhost:7777` hoặc `http://localhost:5000`
- Swagger API: `https://localhost:7777/swagger`

---

## 🏃 CÁCH 2: Chạy bằng VS Code

### Bước 1: Mở Project
1. Mở VS Code
2. **File** → **Open Folder** → Chọn thư mục `e:\AMD`

### Bước 2: Cài Extensions (nếu chưa có)
- **C# Dev Kit** (Microsoft)
- **C#** (Microsoft)

### Bước 3: Restore Packages
Mở Terminal trong VS Code (**Ctrl + `**) và chạy:
```bash
dotnet restore
```

### Bước 4: Apply Database Migrations
```bash
cd src\AMD201.Infrastructure
dotnet ef database update --startup-project ..\AMD201.API
cd ..\..
```

Nếu chưa cài `dotnet ef`, chạy trước:
```bash
dotnet tool install --global dotnet-ef
```

### Bước 5: Build Project
```bash
dotnet build
```

### Bước 6: Run Project
```bash
cd src\AMD201.API
dotnet run
```

### Bước 7: Truy cập ứng dụng
- Mở browser: `https://localhost:7777` hoặc `http://localhost:5000`
- Swagger API: `https://localhost:7777/swagger`

---

## 🏃 CÁCH 3: Chạy nhanh từ Command Line (CMD)

### Nếu đã có database sẵn:
```cmd
cd e:\AMD\src\AMD201.API
dotnet run
```

### Nếu chưa apply migrations:
```cmd
cd e:\AMD
dotnet tool install --global dotnet-ef
cd src\AMD201.Infrastructure
dotnet ef database update --startup-project ..\AMD201.API
cd ..\AMD201.API
dotnet run
```

---

## 🐳 CÁCH 4: Chạy bằng Docker (Optional)

### Build và Run
```bash
docker-compose up --build
```

### Stop
```bash
docker-compose down
```

---

## 📝 Lệnh hữu ích

### Check .NET version
```bash
dotnet --version
```

### Restore packages
```bash
dotnet restore
```

### Build solution
```bash
dotnet build
```

### Run tests
```bash
dotnet test
```

### Clean build artifacts
```bash
dotnet clean
```

### Create new migration
```bash
cd src\AMD201.Infrastructure
dotnet ef migrations add MigrationName --startup-project ..\AMD201.API
```

### Remove last migration
```bash
dotnet ef migrations remove --startup-project ..\AMD201.API
```

### List migrations
```bash
dotnet ef migrations list --startup-project ..\AMD201.API
```

---

## 🌐 URLs sau khi chạy

| Service | URL |
|---------|-----|
| **Web Application** | https://localhost:7777 |
| **HTTP (non-SSL)** | http://localhost:5000 |
| **Swagger API Docs** | https://localhost:7777/swagger |
| **API Base** | https://localhost:7777/api |

---

## 🔍 Kiểm tra Project chạy thành công

### Test 1: Homepage
- Mở: `https://localhost:7777`
- Nên thấy: Giao diện URL Shortener với Vue.js

### Test 2: API Health Check
- Mở: `https://localhost:7777/swagger`
- Nên thấy: Swagger UI với danh sách endpoints

### Test 3: Shorten URL
1. Trên homepage, nhập URL: `https://www.google.com`
2. Click **Shorten URL**
3. Nên nhận được short URL dạng: `https://localhost:7777/abc123`

### Test 4: Redirect
1. Copy short URL vừa tạo
2. Paste vào browser mới
3. Nên redirect về `https://www.google.com`

---

## ❌ Xử lý lỗi thường gặp

### Lỗi: "Unable to connect to database"
**Nguyên nhân**: Database chưa chạy hoặc connection string sai

**Giải pháp**:
- Check connection string trong `appsettings.json`
- Nếu dùng Supabase, check internet connection
- Nếu dùng local PostgreSQL, check service đang chạy

### Lỗi: "Port 5000 already in use"
**Giải pháp**:
- Đổi port trong `launchSettings.json`
- Hoặc kill process đang dùng port:
```bash
netstat -ano | findstr :5000
taskkill /PID <process_id> /F
```

### Lỗi: "dotnet ef command not found"
**Giải pháp**:
```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

### Lỗi: "No migrations found"
**Giải pháp**:
```bash
cd src\AMD201.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ..\AMD201.API
dotnet ef database update --startup-project ..\AMD201.API
```

### Lỗi: "Certificate validation failed" (HTTPS)
**Giải pháp**:
```bash
dotnet dev-certs https --trust
```

### Lỗi: Build failed - NuGet restore
**Giải pháp**:
```bash
dotnet clean
dotnet restore
dotnet build
```

---

## 🔐 Environment Variables (Optional)

Tạo file `appsettings.Development.json` để override settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_LOCAL_CONNECTION_STRING"
  },
  "Supabase": {
    "Url": "YOUR_SUPABASE_URL",
    "Key": "YOUR_SUPABASE_KEY"
  }
}
```

---

## 📱 Chạy Frontend riêng (Development mode)

Nếu muốn edit frontend mà không cần build lại:

1. Frontend đã được serve sẵn từ `wwwroot/index.html`
2. Mỗi lần edit `frontend/index.html`, copy sang `src/AMD201.API/wwwroot/index.html`
3. Hoặc dùng Live Server extension trong VS Code

---

## 🎯 Hot Reload trong Development

### Visual Studio 2022
- Tự động có Hot Reload
- Edit code → Save → Thấy thay đổi ngay

### VS Code
- Install extension: **C# Dev Kit**
- Hoặc dùng `dotnet watch`:
```bash
cd src\AMD201.API
dotnet watch run
```

---

## 📊 Monitoring & Logs

### Xem logs trong Console
- Visual Studio: Output window
- VS Code: Terminal
- CMD: Logs hiện trực tiếp

### Log levels
- `Information`: Normal operations
- `Warning`: Potential issues
- `Error`: Failures
- `Debug`: Detailed info (chỉ trong Development)

---

## 🚀 Production Deployment

### Build for Production
```bash
dotnet publish -c Release -o ./publish
```

### Run production build
```bash
cd publish
dotnet AMD201.API.dll
```

---

## 📞 Hỗ trợ

- Check file `TROUBLESHOOTING.md` để xem thêm solutions
- Check file `PHAN_CONG_TEAM.md` để hiểu cấu trúc code
- Check `README.md` cho overview

---

**Chúc bạn chạy project thành công! 🎉**
