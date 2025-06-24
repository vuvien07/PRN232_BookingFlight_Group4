# Chức năng Đăng ký Người dùng (Customer Registration)

## Tổng quan
Chức năng đăng ký người dùng cho phép khách hàng tạo tài khoản mới với role = 3 (Customer) trong hệ thống đặt vé máy bay.

## Các file đã được tạo/cập nhật

### Backend (BookingFlightServer)
1. **DTO/Request/RegisterDTO.cs** - DTO cho dữ liệu đăng ký
2. **Controllers/RegisterController.cs** - API controller xử lý đăng ký
3. **Services/IAccountService.cs** - Interface được cập nhật với các method đăng ký
4. **Services/Implements/AccountService.cs** - Implementation service đăng ký
5. **Repositories/IAccountRepository.cs** - Interface repository được cập nhật
6. **Repositories/Implements/AccountRepository.cs** - Implementation repository đăng ký

### Frontend (BookingFlightClient)
1. **Controllers/LoginController.cs** - Thêm action Register
2. **Controllers/RegisterController.cs** - Controller mới cho trang đăng ký
3. **Views/Authentication/Register.cshtml** - Trang đăng ký đầy đủ với validation
4. **Views/Authentication/Login.cshtml** - Cập nhật link đến trang đăng ký

### Database
1. **sample_data.sql** - Script tạo dữ liệu mẫu cho Role và Status

## API Endpoints

### POST /api/Register
Đăng ký tài khoản mới
```json
{
  "username": "string",
  "password": "string", 
  "confirmPassword": "string",
  "fullname": "string",
  "address": "string",
  "phoneNumber": "string",
  "email": "string"
}
```

### GET /api/Register/check-username/{username}
Kiểm tra username đã tồn tại chưa

### GET /api/Register/check-email/{email}  
Kiểm tra email đã tồn tại chưa

## Validation Rules

### Username
- Bắt buộc
- Không chứa khoảng trắng
- Tối đa 50 ký tự
- Phải unique

### Password
- Bắt buộc
- Không chứa khoảng trắng
- Từ 6-250 ký tự
- Có kiểm tra độ mạnh password

### Email
- Bắt buộc
- Format email hợp lệ
- Tối đa 50 ký tự
- Phải unique

### Phone Number
- Bắt buộc
- Format: +84909123456 hoặc 0909123456
- Tối đa 17 ký tự

### Full Name
- Bắt buộc
- Tối đa 50 ký tự

### Address
- Bắt buộc
- Tối đa 255 ký tự

## Cách sử dụng

### 1. Chạy Database Script
Chạy file `sample_data.sql` để tạo dữ liệu mẫu cho Role, Status và Permission:
```sql
-- Sẽ tạo:
-- Role: 1=Admin, 2=Manager, 3=Customer, 4=Supporter, 5=Guest  
-- Status: 1=Active, 2=Inactive, 3=Blocked
-- PermissionPage và PermissionAPI cho trang đăng ký
-- Gán quyền Guest để truy cập trang đăng ký
```

### 2. Chạy Server
```bash
cd BookingFlightServer
dotnet run
```
Server sẽ chạy trên http://localhost:5077

### 3. Chạy Client  
```bash
cd BookingFlightClient
dotnet run
```

### 4. Truy cập trang đăng ký
- Trực tiếp: http://localhost:[client-port]/Register
- Từ trang login: Click "Create account"

## Tính năng

### Real-time Validation
- Kiểm tra username/email trùng lặp ngay khi người dùng nhập
- Hiển thị độ mạnh password
- Validation form đầy đủ

### Security
- Password được lưu trực tiếp như người dùng nhập vào
- Validation backend và frontend

### UX/UI
- Giao diện đẹp với Bootstrap 5
- Responsive design
- Modal thông báo lỗi/thành công
- Form validation với hiệu ứng

## Database Schema
```sql
Account table:
- account_id (PK)
- username (unique)
- password (hashed)
- role_id (3 = Customer)
- status_id (1 = Active)

Customer table:  
- customer_id (PK)
- fullname
- address
- phone_number
- email (unique)
- account_id (FK)
```

## Phân quyền (Redis-based Authorization)

Hệ thống sử dụng middleware phân quyền với Redis cache:

### Client Middleware (CheckAccessPageMiddleware)
- Kiểm tra quyền truy cập trang web
- Paths được bỏ qua: `/Unauthorized`, `/Login`, `/Register`, `/Login/Register`
- Role Guest được gán quyền truy cập trang đăng ký

### Server Middleware (CheckAccessApiMiddleware)  
- Kiểm tra quyền truy cập API
- APIs được bỏ qua: `/api/Home`, `/api/login`, `/api/Register`, `/api/Register/check-*`
- Role Guest được gán quyền gọi API đăng ký

### Permissions trong Database
- `PermissionPage`: Định nghĩa quyền truy cập trang
- `PermissionAPI`: Định nghĩa quyền gọi API  
- `RolePermissionPage`: Liên kết Role với Page Permission
- `RolePermissionAPI`: Liên kết Role với API Permission
## Lưu ý
1. Password được lưu trực tiếp như người dùng nhập vào database
2. Role ID = 3 được hard-code cho Customer
3. Status ID = 1 được hard-code cho Active account
4. Hệ thống sử dụng Redis để cache permission - cần chạy Redis server
5. Middleware phân quyền đã được cấu hình để cho phép Guest truy cập trang đăng ký
6. Cần chạy script SQL để tạo permission data trước khi sử dụng
