<div align="center">

# 🎓 Student & Classroom Web API

**RESTful Web API mẫu quản lý Sinh viên & Lớp môn học (Quan hệ Nhiều - Nhiều) xây dựng trên nền tảng .NET 10, Entity Framework Core và Scalar OpenAPI.**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![CI](https://github.com/Tuannll/dotnet-student-classroom-api/actions/workflows/ci.yml/badge.svg)](https://github.com/Tuannll/dotnet-student-classroom-api/actions/workflows/ci.yml)
[![C#](https://img.shields.io/badge/C%23-13-239120?style=flat-square&logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![EF Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4?style=flat-square&logo=nuget&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![Scalar](https://img.shields.io/badge/API%20Docs-Scalar-E83E8C?style=flat-square)](https://scalar.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)

<a id="quick-links"></a>
**[✨ Tính năng](#-tính-năng) • [🚀 Bắt đầu nhanh](#-bắt-đầu-nhanh) • [📡 API Endpoints](#-api-endpoints) • [🛠️ Công nghệ](#️-công-nghệ-sử-dụng) • [❓ FAQ](#-faq)**

</div>

---

## ✨ Tính năng

| Nhóm | Chi tiết |
| --- | --- |
| **Sinh viên (Students)** | CRUD sinh viên, kiểm tra mã số sinh viên duy nhất, quản lý ngày sinh chuẩn định dạng `DateOnly` (`yyyy-MM-dd`). |
| **Lớp học (Classrooms)** | CRUD lớp môn học, ràng buộc mã lớp (tối thiểu 5 ký tự), sĩ số tối đa (`MaxStudents > 0`). |
| **Ghi danh (Enrollment)** | Ghi danh nhiều sinh viên cùng lúc vào lớp (Batch enrollment), tự động kiểm tra trùng lặp và chặn vượt sĩ số tối đa. |
| **Truy vấn LINQ Join** | Lấy danh sách sinh viên theo lớp thông qua phép `join` giữa `StudentClassrooms` và `Students`. |
| **Kiến trúc phân lớp** | Tách bạch rõ ràng `Controller` -> `Service` -> `DbContext` -> `Entities/DTOs`. |
| **Xử lý lỗi 2 lớp** | Bọc `try-catch` tại Controller bắt `UserFriendlyException` kết hợp `GlobalExceptionMiddleware` dự phòng. |
| **Dual Database** | Chạy sẵn `InMemoryDatabase` tự seed dữ liệu mẫu khi clone, chuyển đổi `SQL Server` chỉ bằng 1 dòng cấu hình. |

---

## 🚀 Bắt đầu nhanh

> [!TIP]
> Dự án mặc định sử dụng **InMemory Database** và tự động nạp sẵn dữ liệu mẫu. Bạn có thể clone về và chạy ngay mà không cần cài đặt SQL Server.

### 1. Clone mã nguồn
```bash
git clone https://github.com/Tuannll/dotnet-student-classroom-api.git
cd dotnet-student-classroom-api
```

### 2. Khôi phục packages & Build
```bash
dotnet restore
dotnet build
```

### 3. Chạy ứng dụng
```bash
dotnet run
```

### 4. Truy cập tài liệu API tương tác
Mở trình duyệt truy cập:
- **Scalar API Reference**: `https://localhost:7063/scalar/v1` (hoặc cổng HTTP hiển thị tại console: `http://localhost:5096/scalar/v1`)
- **OpenAPI Schema**: `https://localhost:7063/openapi/v1.json`

---

## 📡 API Endpoints

### Sinh viên (`/api/students`)

| Phương thức | Đường dẫn | Mô tả |
| :--- | :--- | :--- |
| `GET` | `/api/students` | Lấy danh sách toàn bộ sinh viên |
| `GET` | `/api/students/{id}` | Xem chi tiết sinh viên theo ID |
| `GET` | `/api/students/{id}/classrooms` | Xem danh sách các lớp học của sinh viên |
| `POST` | `/api/students` | Thêm sinh viên mới (`dateOfBirth`: `"yyyy-MM-dd"`) |
| `PUT` | `/api/students/{id}` | Cập nhật thông tin sinh viên |
| `DELETE` | `/api/students/{id}` | Xóa sinh viên theo ID |

### Lớp môn học (`/api/classrooms`)

| Phương thức | Đường dẫn | Mô tả |
| :--- | :--- | :--- |
| `GET` | `/api/classrooms` | Lấy danh sách toàn bộ lớp học |
| `GET` | `/api/classrooms/{id}` | Xem chi tiết lớp học theo ID |
| `GET` | `/api/classrooms/{id}/students` | Lấy danh sách sinh viên trong lớp (**LINQ Join**) |
| `POST` | `/api/classrooms` | Thêm mới lớp học |
| `PUT` | `/api/classrooms/{id}` | Cập nhật thông tin lớp học |
| `DELETE` | `/api/classrooms/{id}` | Xóa lớp học (kèm xóa liên kết sinh viên) |
| `POST` | `/api/classrooms/{id}/students` | **Chuẩn REST**: Ghi danh nhiều sinh viên (Body: `[1, 2, 3]`) |
| `POST` | `/api/classrooms/enroll` | Endpoint phụ: Ghi danh bằng `EnrollStudentsDto` |

> [!NOTE]
> File `TH5.http` nằm tại thư mục gốc chứa sẵn **14 kịch bản test request**. Bạn có thể mở trực tiếp trong Visual Studio hoặc VS Code (REST Client Extension) để kiểm thử nhanh.

---

## 💾 Cấu hình Cơ sở dữ liệu

- **Mặc định (InMemory)**: `appsettings.json` để trống `DefaultConnection: ""`. Dữ liệu lưu trên RAM và tự động seed 4 sinh viên, 2 lớp học khi khởi động.
- **Dùng SQL Server thật**: Điền chuỗi kết nối vào `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TH5_StudentClassroomDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## 🛠️ Công nghệ sử dụng

- **Runtime**: .NET 10 (C# 13)
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core 10 (`Microsoft.EntityFrameworkCore.SqlServer`, `InMemory`)
- **API Documentation**: Microsoft.AspNetCore.OpenApi + Scalar.AspNetCore
- **Kiến trúc**: Service Layer Pattern, Generic Response Wrapper (`ApiResponse<T>`)

---

## ❓ FAQ

<details>
<summary><b>1. Làm thế nào để test format ngày sinh?</b></summary>
Kiểu dữ liệu sử dụng là <code>DateOnly</code>. Khi gửi request POST/PUT, chỉ cần truyền chuỗi dạng <code>"2004-11-20"</code> (không kèm giờ, phút, giây hay timezone).
</details>

<details>
<summary><b>2. Khi xóa một lớp học, sinh viên trong lớp có bị mất không?</b></summary>
Không. Trong quan hệ Nhiều - Nhiều, sinh viên có thể tham gia nhiều lớp khác nhau. Khi xóa lớp, hệ thống sử dụng <code>Cascade Delete</code> để xóa các bản ghi liên kết trong bảng <code>StudentClassrooms</code>, thông tin sinh viên tại bảng <code>Students</code> vẫn được giữ nguyên vẹn.
</details>

<details>
<summary><b>3. Logic ghi danh sinh viên xử lý sĩ số như thế nào?</b></summary>
Hệ thống tự động cộng dồn số sinh viên hiện có với số sinh viên mới đăng ký. Nếu vượt quá <code>MaxStudents</code> của lớp, API sẽ trả về lỗi 400 kèm thông báo chi tiết sĩ số hiện tại.
</details>

---

## 📄 License

Dự án phát hành dưới giấy phép [MIT License](LICENSE).
