# Environment Variables Setup

## Database Connection

Ứng dụng sử dụng environment variable `DATABASE_URL` để kết nối database một cách an toàn.

### Format DATABASE_URL

```
postgresql://username:password@host:port/database
```

### Ví dụ:
```
postgresql://sa:password123@dpg-d4om1jq4d50c7390a500-a.singapore-postgres.render.com:5432/productdb_pvn9
```

## Setup trên Render

1. Vào **Dashboard** của service trên Render
2. Chọn **Environment** tab
3. Thêm environment variable:
   - **Key**: `DATABASE_URL`
   - **Value**: Connection string từ PostgreSQL database của bạn (Render tự động tạo khi bạn tạo PostgreSQL database)

Render sẽ tự động cung cấp `DATABASE_URL` nếu bạn:
- Tạo PostgreSQL database trên Render
- Link database với Web Service trong cùng một Blueprint hoặc sử dụng Internal Database URL

## Setup Local Development

### Cách 1: Environment Variables (Khuyến nghị)

Ứng dụng sẽ tự động đọc connection string theo thứ tự ưu tiên:
1. `DATABASE_URL` (format: `postgresql://user:password@host:port/database`)
2. `ConnectionStrings__DefaultConnection` (format Npgsql chuẩn)
3. `DefaultConnection` (format Npgsql chuẩn)
4. `appsettings.Development.json` (fallback cho Visual Studio Server Explorer)

#### Windows (PowerShell)
```powershell
$env:DATABASE_URL="postgresql://username:password@host:port/database"
```

#### Windows (CMD)
```cmd
set DATABASE_URL=postgresql://username:password@host:port/database
```

#### Windows (Environment Variables - Vĩnh viễn)
1. Mở **System Properties** > **Environment Variables**
2. Thêm User Variable:
   - **Name**: `DATABASE_URL` hoặc `ConnectionStrings__DefaultConnection`
   - **Value**: Connection string của bạn

#### Linux/Mac
```bash
export DATABASE_URL="postgresql://username:password@host:port/database"
```

### Cách 2: User Secrets (Bảo mật hơn, nhưng Visual Studio Server Explorer có thể không đọc được)

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=host;Port=5432;Database=db;Username=user;Password=pass"
```

### Cách 3: appsettings.Development.json (Cho Visual Studio Server Explorer)

Để Visual Studio Server Explorer có thể đọc được connection string và hiển thị database:
1. Mở `Product/appsettings.Development.json`
2. Cập nhật connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=your_host;Port=5432;Database=your_db;Username=your_user;Password=your_password;SSL Mode=Require;Trust Server Certificate=true"
     }
   }
   ```

⚠️ **Lưu ý**: 
- Nếu connection string chứa thông tin nhạy cảm, đảm bảo `appsettings.Development.json` được thêm vào `.gitignore` hoặc không commit
- Hoặc chỉ sử dụng connection string localhost (không nhạy cảm)

### Cấu hình Visual Studio Server Explorer

Visual Studio Server Explorer sẽ tự động đọc từ:
1. `appsettings.Development.json` (nếu đang chạy trong Development mode)
2. Environment variables (nếu được set trong Visual Studio)

Để Visual Studio đọc environment variables:
1. Mở project **Properties** > **Debug** > **Environment variables**
2. Thêm: `DATABASE_URL` = `your_connection_string`

## Lưu ý bảo mật

- ✅ **KHÔNG** commit connection string thật vào git
- ✅ **KHÔNG** đặt connection string production trong `appsettings.json`
- ✅ `appsettings.Development.json` có thể chứa connection string localhost (không nhạy cảm)
- ✅ Sử dụng environment variables hoặc User Secrets cho connection string production
- ✅ Trên Render, sử dụng Internal Database URL thay vì Public URL khi có thể
- ✅ Nếu `appsettings.Development.json` chứa thông tin nhạy cảm, thêm vào `.gitignore`

## Kết nối Visual Studio Server Explorer

Visual Studio Server Explorer sẽ tự động kết nối database nếu:
1. Connection string được cấu hình trong `appsettings.Development.json`
2. Hoặc environment variable `ConnectionStrings__DefaultConnection` hoặc `DefaultConnection` được set
3. Database đã được tạo và migrations đã được chạy

Khi mở Server Explorer > Data Connections, Visual Studio sẽ hiển thị các bảng từ database được cấu hình.

