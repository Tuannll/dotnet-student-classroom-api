using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using TH5.Data;
using TH5.Entities;
using TH5.Middleware;
using TH5.Services.Implementations;
using TH5.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers & OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Cấu hình DbContext: ưu tiên SQL Server nếu cấu hình chuỗi kết nối, ngược lại fallback sang InMemory
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TH5_StudentClassroomDb"));
}

// Đăng ký Business Services (Dependency Injection)
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IClassroomService, ClassroomService>();

var app = builder.Build();

// Tự động khởi tạo Database và Seeding dữ liệu mẫu
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.Students.Any() && !context.Classrooms.Any())
    {
        var s1 = new Student { Name = "Nguyen Van A", StudentCode = "SV001", DateOfBirth = new DateOnly(2003, 1, 15) };
        var s2 = new Student { Name = "Tran Thi B", StudentCode = "SV002", DateOfBirth = new DateOnly(2003, 5, 20) };
        var s3 = new Student { Name = "Le Van C", StudentCode = "SV003", DateOfBirth = new DateOnly(2003, 8, 10) };
        var s4 = new Student { Name = "Pham Thi D", StudentCode = "SV004", DateOfBirth = new DateOnly(2004, 2, 28) };
        context.Students.AddRange(s1, s2, s3, s4);

        var c1 = new Classroom { Name = "Backend Development C#", ClassCode = "CNTT-BE01", MaxStudents = 30 };
        var c2 = new Classroom { Name = "Frontend Development React", ClassCode = "CNTT-FE01", MaxStudents = 25 };
        context.Classrooms.AddRange(c1, c2);

        context.SaveChanges();

        // Ghi danh mẫu
        context.StudentClassrooms.AddRange(
            new StudentClassroom { StudentId = s1.Id, ClassroomId = c1.Id },
            new StudentClassroom { StudentId = s2.Id, ClassroomId = c1.Id },
            new StudentClassroom { StudentId = s1.Id, ClassroomId = c2.Id }
        );
        context.SaveChanges();
    }
}

// Global Exception Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("TH5 - Many-to-Many Student & Classroom API");
        options.WithTheme(ScalarTheme.Mars);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
