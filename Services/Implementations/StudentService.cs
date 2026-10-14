using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TH5.Data;
using TH5.Dtos;
using TH5.Entities;
using TH5.Exceptions;
using TH5.Services.Interfaces;

namespace TH5.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentResponseDto>> GetAllAsync()
        {
            return await _context.Students
                .AsNoTracking()
                .Select(s => new StudentResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    StudentCode = s.StudentCode,
                    DateOfBirth = s.DateOfBirth
                })
                .ToListAsync();
        }

        public async Task<StudentResponseDto> GetByIdAsync(int id)
        {
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                throw new UserFriendlyException($"Không tìm thấy sinh viên có ID = {id}", 404);
            }

            return new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                StudentCode = student.StudentCode,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
        {
            var exists = await _context.Students.AnyAsync(s => s.StudentCode == dto.StudentCode.Trim());
            if (exists)
            {
                throw new UserFriendlyException($"Mã sinh viên '{dto.StudentCode.Trim()}' đã tồn tại trong hệ thống.", 400);
            }

            var student = new Student
            {
                Name = dto.Name.Trim(),
                StudentCode = dto.StudentCode.Trim(),
                DateOfBirth = dto.DateOfBirth
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                StudentCode = student.StudentCode,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task<StudentResponseDto> UpdateAsync(int id, UpdateStudentDto dto)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                throw new UserFriendlyException($"Không tìm thấy sinh viên có ID = {id}", 404);
            }

            var duplicateCode = await _context.Students
                .AnyAsync(s => s.Id != id && s.StudentCode == dto.StudentCode.Trim());
            if (duplicateCode)
            {
                throw new UserFriendlyException($"Mã sinh viên '{dto.StudentCode.Trim()}' đã thuộc về sinh viên khác.", 400);
            }

            student.Name = dto.Name.Trim();
            student.StudentCode = dto.StudentCode.Trim();
            student.DateOfBirth = dto.DateOfBirth;

            await _context.SaveChangesAsync();

            return new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                StudentCode = student.StudentCode,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task DeleteAsync(int id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                throw new UserFriendlyException($"Không tìm thấy sinh viên có ID = {id}", 404);
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public async Task<StudentClassroomsResponseDto> GetClassroomsOfStudentAsync(int studentId)
        {
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                throw new UserFriendlyException($"Không tìm thấy sinh viên có ID = {studentId}", 404);
            }

            // LINQ Join qua bang trung gian StudentClassrooms va Classrooms
            var classrooms = await (from sc in _context.StudentClassrooms.AsNoTracking()
                                    where sc.StudentId == studentId
                                    join c in _context.Classrooms.AsNoTracking() on sc.ClassroomId equals c.Id
                                    select new ClassroomResponseDto
                                    {
                                        Id = c.Id,
                                        Name = c.Name,
                                        ClassCode = c.ClassCode,
                                        MaxStudents = c.MaxStudents,
                                        CurrentStudentCount = c.StudentClassrooms.Count
                                    }).ToListAsync();

            return new StudentClassroomsResponseDto
            {
                StudentId = student.Id,
                StudentName = student.Name,
                StudentCode = student.StudentCode,
                Classrooms = classrooms
            };
        }
    }
}
