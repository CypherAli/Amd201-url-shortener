# 📋 PHÂN CÔNG CÔNG VIỆC TEAM - URL SHORTENER PROJECT

## 👥 Thành viên Team (3 người)
- **Frontend Developer** (1 người)
- **Backend Developer 1** (1 người) 
- **Backend Developer 2** (1 người)

---

## 🎨 FRONTEND DEVELOPER

### Trách nhiệm chính
Phát triển giao diện người dùng Single Page Application (SPA) với Vue.js

### Files phụ trách
- `frontend/index.html` - Toàn bộ UI/UX application

### Chi tiết công việc đã làm

#### 1. **UI/UX Design & Layout**
- Thiết kế responsive layout với gradient background đẹp mắt
- Tạo component-based structure với Vue.js 3
- Implement dark/light theme với color scheme chuyên nghiệp

#### 2. **Authentication UI**
- Modal đăng nhập/đăng ký với Email/Password
- Tích hợp Social Login buttons (Discord, GitHub)
- Form validation và error handling
- Toggle giữa Sign In/Sign Up mode

#### 3. **Main Features UI**
- **Form rút gọn URL**:
  - Input field cho Long URL
  - Custom short code input (chỉ hiện khi đã login)
  - Real-time check availability của custom code
  - Copy to clipboard button
  - Success/Error notifications

- **Tab System**:
  - Tab "Shorten URL" để tạo link mới
  - Tab "My URLs" hiển thị lịch sử (khi đã login)
  
- **URL History Management**:
  - Hiển thị danh sách URLs đã tạo
  - Thông tin: short code, original URL, click count, created date
  - Edit và Delete buttons cho mỗi URL
  - Badge hiển thị "Custom" cho custom links

#### 4. **State Management**
- Quản lý user authentication state
- Sync với Supabase Auth
- Loading states và error handling
- Toast notifications (auto-dismiss sau 5s)

#### 5. **API Integration**
- Kết nối với Backend API endpoints:
  - `POST /api/url/shorten` - Tạo short URL
  - `GET /api/url/history` - Lấy lịch sử URLs
  - `GET /api/url/check/{code}` - Kiểm tra custom code
  - `PUT /api/url/{code}` - Cập nhật URL
  - `DELETE /api/url/{code}` - Xóa URL
- Header Authorization với Supabase JWT token
- Error handling và user feedback

#### 6. **Edit Mode Feature**
- Edit existing URLs inline
- Change original URL hoặc custom code
- Generate random code button
- Cancel edit functionality

### Demo Points khi thầy hỏi
1. "Em phụ trách toàn bộ phần Frontend với Vue.js"
2. "Em làm responsive design với gradient background này"
3. "Em implement authentication UI với Social Login (Discord, GitHub) và Email/Password"
4. "Em tạo form rút gọn URL với real-time validation"
5. "Em làm tính năng quản lý URLs: xem lịch sử, edit, delete"
6. "Em tích hợp Supabase Authentication để đồng bộ user session"
7. "Em handle API calls với async/await và error handling"

---

## ⚙️ BACKEND DEVELOPER 1

### Trách nhiệm chính
API Layer & Controllers - Xử lý HTTP requests/responses

### Files phụ trách
- `src/AMD201.API/Program.cs` - Configuration & Middleware setup
- `src/AMD201.API/Controllers/UrlController.cs` - REST API cho URL operations
- `src/AMD201.API/Controllers/RedirectController.cs` - Redirect logic
- `src/AMD201.API/Middleware/SupabaseAuthMiddleware.cs` - Authentication middleware

### Chi tiết công việc đã làm

#### 1. **Program.cs - Application Setup**
- Configure Dependency Injection:
  - Register DbContext với PostgreSQL
  - Register Services (IUrlShortenerService, IUrlRepository)
  - Configure CORS policy
- Setup Middleware pipeline:
  - Supabase Authentication Middleware
  - Static files serving
  - HTTPS redirection
- Configure Swagger/OpenAPI documentation
- Setup Entity Framework Migrations

#### 2. **UrlController.cs - Main API Endpoints**

**POST /api/url/shorten**
- Nhận request: `originalUrl`, `customCode` (optional), `expiresInDays` (optional)
- Extract userId từ authentication
- Validate input với try-catch
- Gọi Service layer để xử lý
- Return shortened URL với proper status codes (200, 400, 401, 409, 500)

**GET /api/url/history**
- Require authentication
- Pagination với `page` và `pageSize` parameters
- Lấy danh sách URLs của user
- Return: list URLs, total count, total clicks

**GET /api/url/stats/{shortCode}**
- Lấy thống kê chi tiết cho một URL
- Authorization check: chỉ owner mới xem được stats
- Return: click count, recent clicks với geo data

**GET /api/url/check/{shortCode}**
- Kiểm tra custom code có available không
- Public endpoint (không cần auth)
- Return: `{ shortCode, available: true/false }`

**PUT /api/url/{shortCode}**
- Update existing URL
- Require authentication
- Cho phép đổi originalUrl hoặc customCode
- Generate random code nếu requested
- Validation và conflict handling

**DELETE /api/url/{shortCode}**
- Xóa URL
- Require authentication
- Authorization: chỉ owner mới được xóa
- Return 404 nếu không tìm thấy hoặc không có quyền

#### 3. **RedirectController.cs**

**GET /{shortCode}**
- Public endpoint cho redirect
- Lấy original URL từ database
- Kiểm tra expiration
- Fire-and-forget tracking (không chờ save stats)
- HTTP 302 Redirect
- Error handling với proper status codes

#### 4. **SupabaseAuthMiddleware.cs**
- Intercept mọi HTTP requests
- Extract JWT token từ Authorization header
- Validate token với Supabase
- Extract userId và attach vào HttpContext.Items
- Handle expired/invalid tokens
- Cho phép anonymous access (không require auth cho mọi endpoint)

#### 5. **Error Handling & Logging**
- Try-catch blocks trong mọi endpoints
- Log errors với ILogger
- Return user-friendly error messages
- Proper HTTP status codes:
  - 200: Success
  - 400: Bad Request (invalid input)
  - 401: Unauthorized
  - 404: Not Found
  - 409: Conflict (duplicate custom code)
  - 500: Internal Server Error

### Demo Points khi thầy hỏi
1. "Em phụ trách API Layer - Controllers và Middleware"
2. "Em setup Program.cs: DI container, CORS, Swagger, EF Core"
3. "Em implement UrlController với 6 endpoints: shorten, history, stats, check, update, delete"
4. "Em làm RedirectController xử lý redirect từ short code về original URL"
5. "Em tạo SupabaseAuthMiddleware để validate JWT token và extract user info"
6. "Em handle authentication/authorization: phân biệt anonymous vs logged-in users"
7. "Em implement error handling đầy đủ với try-catch và logging"
8. "Em làm fire-and-forget pattern cho tracking để không slow down redirect"

---

## 💾 BACKEND DEVELOPER 2

### Trách nhiệm chính
Business Logic & Data Access Layer

### Files phụ trách
- `src/AMD201.Core/` - Domain Models & Interfaces
  - `Entities/ShortenedUrl.cs`
  - `Entities/ClickStatistic.cs`
  - `DTOs/UrlDtos.cs`
  - `Interfaces/IUrlShortenerService.cs`
  - `Interfaces/IUrlRepository.cs`
  
- `src/AMD201.Infrastructure/` - Implementation
  - `Services/UrlShortenerService.cs`
  - `Repositories/UrlRepository.cs`
  - `Data/ApplicationDbContext.cs`
  - `Migrations/20251118000000_InitialCreate.cs`

- `tests/AMD201.Tests/Services/UrlShortenerServiceTests.cs`

### Chi tiết công việc đã làm

#### 1. **Domain Entities (Core Layer)**

**ShortenedUrl.cs**
- Properties: Id, ShortCode, OriginalUrl, UserId, CreatedAt, ExpiresAt, IsActive, IsCustom, ClickCount
- Navigation property: ClickStatistics
- Represent business object trong database

**ClickStatistic.cs**
- Track analytics: IpAddress, UserAgent, Referrer, Country, City, ClickedAt
- Foreign key: ShortenedUrlId
- Collect data cho reporting

**UrlDtos.cs**
- Request DTOs: `ShortenUrlRequest`, `UpdateUrlRequest`
- Response DTOs: `ShortenUrlResponse`, `UserHistoryResponse`, `UrlStatisticsResponse`
- Clean separation giữa API contracts và domain models

#### 2. **Interfaces (Core Layer)**

**IUrlShortenerService.cs**
- Business logic contract:
  - `ShortenUrlAsync()` - Generate short URL
  - `GetOriginalUrlAsync()` - Resolve short code
  - `IncrementClickCountAsync()` - Track analytics
  - `GetUrlStatisticsAsync()` - Get stats
  - `GetUserHistoryAsync()` - User's URLs
  - `IsShortCodeAvailableAsync()` - Check availability
  - `DeleteUrlAsync()` - Remove URL
  - `UpdateUrlAsync()` - Modify URL

**IUrlRepository.cs**
- Data access contract:
  - CRUD operations
  - Query by shortCode, userId
  - Click statistics management
  - Pagination support

#### 3. **UrlShortenerService.cs - Core Business Logic**

**ShortenUrlAsync()**
- Validate URL format (HTTP/HTTPS only)
- Custom code logic:
  - Only for authenticated users
  - Validate format (3-20 chars, alphanumeric + hyphens)
  - Check duplicates
- Random code generation:
  - Use base62 alphabet (a-zA-Z0-9)
  - 6 character default length
  - Collision detection với retry logic
  - Auto-increase length nếu quá nhiều collisions
- Handle expiration dates
- Return formatted response với full short URL

**GetOriginalUrlAsync()**
- Lookup by short code
- Check expiration
- Return null nếu expired hoặc không tồn tại

**IncrementClickCountAsync()**
- Atomic counter increment
- Save click analytics (IP, User-Agent, Referrer)
- Non-blocking để không slow redirect

**GetUrlStatisticsAsync()**
- Authorization check: chỉ owner xem được
- Aggregate total clicks
- Fetch recent 100 clicks với geo data
- Return formatted statistics

**UpdateUrlAsync()**
- Authorization check
- Update original URL nếu provided
- Change custom code:
  - Validate new code
  - Check availability
  - Update IsCustom flag
- Generate random code option
- Preserve click count và analytics

**DeleteUrlAsync()**
- Authorization: chỉ owner được xóa
- Soft delete hoặc hard delete (tùy business requirement)

**Helper Methods**
- `GenerateUniqueShortCodeAsync()` - With collision avoidance
- `GenerateRandomCode()` - Base62 random string
- `IsValidUrl()` - URI format validation
- `IsValidCustomCode()` - Custom code rules

#### 4. **UrlRepository.cs - Data Access**

**Query Methods**
- `GetByShortCodeAsync()` - Include ClickStatistics
- `GetByUserIdAsync()` - Pagination support
- `GetCountByUserIdAsync()` - Total count
- `ExistsAsync()` - Duplicate check

**CRUD Operations**
- `AddAsync()` - Insert new URL
- `UpdateAsync()` - Modify existing
- `DeleteAsync()` - Remove URL

**Analytics**
- `AddClickStatisticAsync()` - Save click data
- `GetClickStatisticsByUrlIdAsync()` - Recent clicks with limit

**Entity Framework Features**
- `.Include()` for eager loading
- Async/await patterns
- LINQ queries
- Change tracking

#### 5. **ApplicationDbContext.cs**

**DbSets**
- `ShortenedUrls` - Main table
- `ClickStatistics` - Analytics table

**OnModelCreating()**
- Fluent API configuration:
  - Primary keys
  - Indexes (ShortCode for fast lookup)
  - Foreign key relationships
  - Column constraints (MaxLength, Required)
  - Default values
- Seed data nếu cần

#### 6. **Database Migration**

**20251118000000_InitialCreate.cs**
- Up(): Create tables schema
- Down(): Rollback migration
- Index creation cho performance
- Foreign key constraints

#### 7. **Unit Tests**

**UrlShortenerServiceTests.cs**
- Test custom code validation
- Test random code generation (uniqueness)
- Mock IUrlRepository
- Test expiration logic
- Test authorization logic
- Edge cases và error handling

### Demo Points khi thầy hỏi
1. "Em phụ trách Business Logic và Data Access Layer"
2. "Em thiết kế Domain Models: ShortenedUrl, ClickStatistic với relationships"
3. "Em tạo DTOs để separate API contracts khỏi database entities"
4. "Em implement UrlShortenerService với toàn bộ business logic:"
   - "Random code generation algorithm (base62, collision avoidance)"
   - "Custom code validation rules"
   - "URL expiration handling"
   - "Authorization checks"
5. "Em implement Repository pattern với Entity Framework Core:"
   - "CRUD operations"
   - "Query optimization với Include()"
   - "Pagination support"
6. "Em setup ApplicationDbContext và Entity Framework configuration"
7. "Em tạo Database Migration để init schema"
8. "Em viết Unit Tests cho Service layer với mocking"
9. "Em design analytics tracking system (IP, User-Agent, Geo location)"

---

## 📊 Cấu trúc Project (Clean Architecture)

```
├── frontend/                    [Frontend Dev]
│   └── index.html              - Vue.js SPA
│
├── src/
│   ├── AMD201.API/             [Backend Dev 1]
│   │   ├── Controllers/
│   │   │   ├── UrlController.cs       - REST API endpoints
│   │   │   └── RedirectController.cs  - Redirect logic
│   │   ├── Middleware/
│   │   │   └── SupabaseAuthMiddleware.cs
│   │   └── Program.cs                 - App configuration
│   │
│   ├── AMD201.Core/            [Backend Dev 2]
│   │   ├── Entities/
│   │   │   ├── ShortenedUrl.cs       - Domain model
│   │   │   └── ClickStatistic.cs     - Analytics model
│   │   ├── DTOs/
│   │   │   └── UrlDtos.cs            - Data transfer objects
│   │   └── Interfaces/
│   │       ├── IUrlShortenerService.cs
│   │       └── IUrlRepository.cs
│   │
│   └── AMD201.Infrastructure/  [Backend Dev 2]
│       ├── Services/
│       │   └── UrlShortenerService.cs - Business logic
│       ├── Repositories/
│       │   └── UrlRepository.cs      - Data access
│       ├── Data/
│       │   └── ApplicationDbContext.cs
│       └── Migrations/
│           └── 20251118000000_InitialCreate.cs
│
└── tests/                      [Backend Dev 2]
    └── AMD201.Tests/
        └── Services/
            └── UrlShortenerServiceTests.cs
```

---

## 🎯 Features Overview

### Core Features
✅ **URL Shortening** - Random hoặc custom short codes  
✅ **Authentication** - Email/Password + Social Login (Discord, GitHub)  
✅ **Authorization** - Public access + User-specific features  
✅ **URL Management** - Create, Read, Update, Delete  
✅ **Analytics** - Click tracking với geo data  
✅ **Expiration** - Optional expiry dates  
✅ **Real-time Validation** - Check custom code availability  

### Technical Stack
- **Frontend**: Vue.js 3, Supabase Auth Client
- **Backend**: ASP.NET Core 9.0, Entity Framework Core
- **Database**: PostgreSQL (via Supabase)
- **Authentication**: Supabase Auth (JWT)
- **Architecture**: Clean Architecture (3-layer)
- **Testing**: xUnit, Moq

---

## 💡 Tips khi Demo với Thầy

### Cho Frontend Developer
- Mở browser DevTools để show Network tab (API calls)
- Demo responsive design bằng cách resize window
- Show validation messages (real-time check)
- Demo authentication flow (login/logout)
- Copy to clipboard feature

### Cho Backend Developer 1 (API Layer)
- Mở Swagger UI (`/swagger`) để show API documentation
- Dùng Postman/Thunder Client test endpoints
- Show logs trong terminal
- Explain HTTP status codes
- Demo authentication với/không có token

### Cho Backend Developer 2 (Logic Layer)
- Mở database viewer (DBeaver, pgAdmin) show tables
- Explain entity relationships
- Show migration files
- Run unit tests (`dotnet test`)
- Explain algorithm (base62, collision handling)

### Common Questions & Answers

**Q: "Em làm phần nào?"**
- Xem section tương ứng phía trên

**Q: "Giải thích flow hoạt động?"**
- User nhập URL → Frontend gửi request
- Backend Dev 1: Controller nhận request, validate
- Backend Dev 2: Service xử lý logic, Repository save DB
- Backend Dev 1: Controller return response
- Frontend: Hiển thị kết quả cho user

**Q: "Có khó khăn gì không?"**
- Frontend: "State management với async operations"
- Backend 1: "Authentication middleware và error handling"
- Backend 2: "Collision avoidance trong short code generation"

**Q: "Có làm việc nhóm như thế nào?"**
- "Team họp define interfaces (DTOs, API endpoints)"
- "Frontend làm mock data trước, Backend Dev 1 làm Controllers"
- "Backend Dev 2 implement logic song song"
- "Tích hợp và test chéo với nhau"

---

## ✅ Checklist Demo Day

### Chuẩn bị
- [ ] Database có data mẫu (một vài URLs)
- [ ] Project chạy được (dotnet run)
- [ ] Browser đã login sẵn một account
- [ ] Code đã commit lên Git
- [ ] Swagger UI accessible

### Scenarios Demo
1. **Scenario 1: Anonymous User**
   - Mở trang chủ (chưa login)
   - Shorten một URL
   - Click vào short URL → redirect
   
2. **Scenario 2: Authenticated User**
   - Login với Email/Password
   - Tạo custom short URL
   - View history
   - Edit một URL
   - Delete một URL
   
3. **Scenario 3: Developer View**
   - Show code structure
   - Explain architecture layers
   - Show database schema
   - Run unit tests

---

**Chúc team demo thành công! 🎉**
