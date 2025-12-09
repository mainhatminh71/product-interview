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

### Windows (PowerShell)
```powershell
$env:DATABASE_URL="postgresql://username:password@host:port/database"
```

### Windows (CMD)
```cmd
set DATABASE_URL=postgresql://username:password@host:port/database
```

### Linux/Mac
```bash
export DATABASE_URL="postgresql://username:password@host:port/database"
```

### Hoặc sử dụng .env file (nếu dùng dotnet user-secrets)
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=host;Port=5432;Database=db;Username=user;Password=pass"
```

## Lưu ý bảo mật

- ✅ **KHÔNG** commit connection string thật vào git
- ✅ **KHÔNG** đặt connection string trong `appsettings.json`
- ✅ Sử dụng environment variables hoặc secrets management
- ✅ Trên Render, sử dụng Internal Database URL thay vì Public URL khi có thể

