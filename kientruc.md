HỆ THỐNG AUTHENTICATION & AUTHORIZATION
1. Mục đích hệ thống

Hệ thống cung cấp dịch vụ xác thực (authentication) tập trung cho các service khác trong toàn bộ hệ sinh thái, sử dụng Keycloak để lưu trữ và xác thực thông tin user.
Toàn bộ nghiệp vụ phân quyền (authorization), bao gồm quản lý role và permission, được thực hiện và lưu trữ trong hệ thống nội bộ, không sử dụng chức năng role/permission của Keycloak.
2. Chức năng chính

    Xác thực (Authentication):
        Nhận yêu cầu đăng nhập từ các service hoặc người dùng.
        Giao tiếp với Keycloak để xác thực thông tin đăng nhập.
        Trả về access token (JWT) cho client/service.

    Quản lý user:
        Tạo, cập nhật, xóa user, role, permission
        Khi có thay đổi về user, đồng bộ thông tin user lên Keycloak (chỉ thông tin user, không đồng bộ role/permission).

    Phân quyền (Authorization):
        Quản lý, lưu trữ role và permission trong hệ thống nội bộ (PostgreSQL).
        Gán role/permission cho user trong hệ thống nội bộ.
        Khi xác thực, lấy thông tin user từ Keycloak, sau đó lấy role/permission từ hệ thống nội bộ để kiểm tra quyền truy cập tài nguyên.

    Đồng bộ hóa với Keycloak:
        Khi có thay đổi về user, hệ thống sẽ đồng bộ với Keycloak qua API (chỉ thông tin user).
        Không sử dụng chức năng role/permission của Keycloak.

3. Yêu cầu kỹ thuật

    Ngôn ngữ: .NET 9 (C#)
    Kiến trúc: Clean Architecture
    API: FastEndpoints
    CQRS & Mediator: MediatR
    Database: PostgreSQL (EF Core)
    Authentication: Keycloak (chỉ lưu user, xác thực user)
    Authorization: Quản lý role/permission nội bộ
    Pattern: UnitOfWork & Repository
    Đồng bộ user: Sử dụng Keycloak Admin REST API
    Quản lý DI: Microsoft Dependency Injection
    Testing: xUnit/NUnit, Moq

4. Sơ đồ kiến trúc tổng thể

Copy

+-------------------+
|    Presentation   |  (API - FastEndpoints)
+-------------------+
           |
           v
+-------------------+
|    Application    |  (CQRS, MediatR Handlers, Use Cases)
+-------------------+
           |
           v
+-------------------+
|     Domain        |  (Entities, Aggregates, Interfaces)
+-------------------+
           |
           v
+-------------------+
|  Infrastructure   |  (Persistence, Repositories, Keycloak, External Services)
+-------------------+
           |
           v
+-------------------+
|   PostgreSQL DB   |  (Lưu user mapping, role, permission)
+-------------------+
           |
           v
+-------------------+
|     Keycloak      |  (Chỉ lưu user, xác thực user)
+-------------------+

5. Cấu trúc thư mục

Copy

/ProjectRoot
│
├── src
│   ├── Domain
│   │   ├── Entities
│   │   │   ├── User.cs
│   │   │   ├── Role.cs
│   │   │   └── Permission.cs
│   │   └── Interfaces
│   │       └── Repositories
│   │           ├── IUserRepository.cs
│   │           ├── IRoleRepository.cs
│   │           └── IPermissionRepository.cs
│   ├── Application
│   │   ├── Interfaces
│   │   │   ├── IUserService.cs
│   │   │   ├── IRoleService.cs
│   │   │   └── IPermissionService.cs
│   │   └── Features
│   │       ├── users
│   │       │   ├── Commands
│   │       │   ├── Queries
│   │       │   └── Dto
│   │       ├── roles
│   │       │   ├── Commands
│   │       │   ├── Queries
│   │       │   └── Dto
│   │       └── permissions
│   │           ├── Commands
│   │           ├── Queries
│   │           └── Dto
│   ├── Infrastructure
│   │   ├── Persistence
│   │   │   ├── DbContext.cs
│   │   │   ├── UnitOfWork.cs
│   │   │   └── Configuration
│   │   ├── Repositories
│   │   │   ├── UserRepository.cs
│   │   │   ├── RoleRepository.cs
│   │   │   └── PermissionRepository.cs
│   │   └── Keycloak
│   │       └── KeycloakService.cs
│   ├── API
│   │   ├── Endpoints
│   │   │   ├── users
│   │   │   ├── roles
│   │   │   └── permissions
│   │   └── Program.cs
│   └── Shared
│
├── tests
│   ├── Application.Tests
│   ├── Infrastructure.Tests
│   └── API.Tests
│
└── README.md (file này)

6. Luồng hoạt động tiêu biểu
Đăng nhập (Login)

    Client gửi yêu cầu đăng nhập tới API.
    API chuyển tiếp thông tin đăng nhập tới Keycloak để xác thực.
    Nếu thành công, Keycloak trả về access token; API trả token này cho client/service.

Kiểm tra quyền truy cập (Authorization)

    Service gửi request kèm token tới API.
    API xác thực token với Keycloak.
    API lấy thông tin user từ Keycloak (hoặc mapping user nội bộ).
    API lấy role/permission của user từ hệ thống nội bộ (PostgreSQL).
    Kiểm tra quyền truy cập tài nguyên dựa trên role/permission nội bộ.

Tạo user mới

    Admin gửi yêu cầu tạo user tới API.
    API gọi Application Layer (qua MediatR) để xử lý nghiệp vụ tạo user.
    Application Layer tạo user trong DB nội bộ, đồng thời gọi KeycloakService để đồng bộ user lên Keycloak (chỉ thông tin user).
    Gán role/permission cho user trong hệ thống nội bộ.

7. Ghi chú triển khai

    Không sử dụng chức năng role/permission của Keycloak.
    Toàn bộ nghiệp vụ phân quyền, kiểm tra quyền, mapping role/permission đều thực hiện trong hệ thống nội bộ.
    Khi xác thực, chỉ sử dụng Keycloak để xác thực user và lấy thông tin user.
    Khi có thay đổi về user, luôn đồng bộ với Keycloak để đảm bảo nhất quán thông tin user.
    Có thể mở rộng thêm các module quản lý nhóm quyền, policy, audit log, v.v.
