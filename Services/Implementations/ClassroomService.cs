using System;
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
    public class ClassroomService : IClassroomService
    {
        private readonly AppDbContext _context;

        public ClassroomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClassroomResponseDto>> GetAllAsync()
        {
            return await _context.Classrooms
                .AsNoTracking()
                .Select(c => new ClassroomResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ClassCode = c.ClassCode,
                    MaxStudents = c.MaxStudents,
                    CurrentStudentCount = c.StudentClassrooms.Count
                })
                .ToListAsync();
        }

        public async Task<ClassroomResponseDto> GetByIdAsync(int id)
        {
            var classroom = await _context.Classrooms
                .AsNoTracking()
                .Select(c => new ClassroomResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ClassCode = c.ClassCode,
                    MaxStudents = c.MaxStudents,
                    CurrentStudentCount = c.StudentClassrooms.Count
                })
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classroom == null)
            {
                throw new UserFriendlyException($"Không tìm thấy lớp học có ID = {id}", 404);
            }

            return classroom;
        }

        public async Task<ClassroomResponseDto> CreateAsync(CreateClassroomDto dto)
        {
            var exists = await _context.Classrooms.AnyAsync(c => c.ClassCode == dto.ClassCode.Trim());
            if (exists)
            {
                throw new UserFriendlyException($"Mã lớp '{dto.ClassCode.Trim()}' đã tồn tại trong hệ thống.", 400);
            }

            var classroom = new Classroom
            {
                Name = dto.Name.Trim(),
                ClassCode = dto.ClassCode.Trim(),
                MaxStudents = dto.MaxStudents
            };

            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();

            return new ClassroomResponseDto
            {
                Id = classroom.Id,
                Name = classroom.Name,
                ClassCode = classroom.ClassCode,
                MaxStudents = classroom.MaxStudents,
                CurrentStudentCount = 0
            };
        }

        public async Task<ClassroomResponseDto> UpdateAsync(int id, UpdateClassroomDto dto)
        {
            var classroom = await _context.Classrooms
                .Include(c => c.StudentClassrooms)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classroom == null)
            {
                throw new UserFriendlyException($"Không tìm thấy lớp học có ID = {id}", 404);
            }

            var duplicateCode = await _context.Classrooms
                .AnyAsync(c => c.Id != id && c.ClassCode == dto.ClassCode.Trim());
            if (duplicateCode)
            {
                throw new UserFriendlyException($"Mã lớp '{dto.ClassCode.Trim()}' đã thuộc về lớp học khác.", 400);
            }

            if (dto.MaxStudents < classroom.StudentClassrooms.Count)
            {
                throw new UserFriendlyException($"Không thể giảm sĩ số tối đa xuống {dto.MaxStudents} vì lớp hiện có {classroom.StudentClassrooms.Count} sinh viên.", 400);
            }

            classroom.Name = dto.Name.Trim();
            classroom.ClassCode = dto.ClassCode.Trim();
            classroom.MaxStudents = dto.MaxStudents;

            await _context.SaveChangesAsync();

            return new ClassroomResponseDto
            {
                Id = classroom.Id,
                Name = classroom.Name,
                ClassCode = classroom.ClassCode,
                MaxStudents = classroom.MaxStudents,
                CurrentStudentCount = classroom.StudentClassrooms.Count
            };
        }

        public async Task DeleteAsync(int id)
        {
            var classroom = await _context.Classrooms.FirstOrDefaultAsync(c => c.Id == id);
            if (classroom == null)
            {
                throw new UserFriendlyException($"Không tìm thấy lớp học có ID = {id}", 404);
            }

            _context.Classrooms.Remove(classroom);
            await _context.SaveChangesAsync();
        }

        public async Task<ClassroomStudentsResponseDto> GetStudentsInClassroomAsync(int classroomId)
        {
            var classroom = await _context.Classrooms
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == classroomId);

            if (classroom == null)
            {
                throw new UserFriendlyException($"Không tìm thấy lớp học có ID = {classroomId}", 404);
            }

            // LINQ Join qua bang trung gian StudentClassrooms va Students
            var students = await (from sc in _context.StudentClassrooms.AsNoTracking()
                                  where sc.ClassroomId == classroomId
                                  join s in _context.Students.AsNoTracking() on sc.StudentId equals s.Id
                                  select new StudentResponseDto
                                  {
                                      Id = s.Id,
                                      Name = s.Name,
                                      StudentCode = s.StudentCode,
                                      DateOfBirth = s.DateOfBirth
                                  }).ToListAsync();

            return new ClassroomStudentsResponseDto
            {
                ClassroomId = classroom.Id,
                ClassroomName = classroom.Name,
                ClassCode = classroom.ClassCode,
                MaxStudents = classroom.MaxStudents,
                Students = students
            };
        }

        public async Task<ClassroomStudentsResponseDto> EnrollStudentsAsync(int classroomId, List<int> studentIds)
        {
            if (studentIds == null || studentIds.Count == 0)
            {
                throw new UserFriendlyException("Danh sách sinh viên ghi danh phải có ít nhất 1 sinh viên.", 400);
            }

            var classroom = await _context.Classrooms
                .Include(c => c.StudentClassrooms)
                .FirstOrDefaultAsync(c => c.Id == classroomId);

            if (classroom == null)
            {
                throw new UserFriendlyException($"Không tìm thấy lớp học có ID = {classroomId}", 404);
            }

            var distinctStudentIds = studentIds.Distinct().ToList();

            // Kiểm tra các sinh viên có tồn tại trong hệ thống không
            var existingStudents = await _context.Students
                .AsNoTracking()
                .Where(s => distinctStudentIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            var missingIds = distinctStudentIds.Except(existingStudents).ToList();
            if (missingIds.Count > 0)
            {
                throw new UserFriendlyException($"Các mã sinh viên sau không tồn tại trong hệ thống: {string.Join(", ", missingIds)}", 400);
            }

            // Lấy danh sách ID đã có trong lớp
            var enrolledIds = classroom.StudentClassrooms.Select(sc => sc.StudentId).ToHashSet();
            var alreadyEnrolled = distinctStudentIds.Where(id => enrolledIds.Contains(id)).ToList();
            if (alreadyEnrolled.Count > 0)
            {
                throw new UserFriendlyException($"Các sinh viên sau đã có trong lớp học: {string.Join(", ", alreadyEnrolled)}", 400);
            }

            // Kiểm tra sĩ số tối đa
            if (classroom.StudentClassrooms.Count + distinctStudentIds.Count > classroom.MaxStudents)
            {
                throw new UserFriendlyException($"Vượt quá sĩ số tối đa của lớp! Lớp hiện có {classroom.StudentClassrooms.Count}/{classroom.MaxStudents} sinh viên, không thể thêm {distinctStudentIds.Count} sinh viên.", 400);
            }

            foreach (var studentId in distinctStudentIds)
            {
                _context.StudentClassrooms.Add(new StudentClassroom
                {
                    ClassroomId = classroom.Id,
                    StudentId = studentId
                });
            }

            await _context.SaveChangesAsync();

            return await GetStudentsInClassroomAsync(classroom.Id);
        }

        public async Task<ClassroomStudentsResponseDto> EnrollStudentsAsync(EnrollStudentsDto dto)
        {
            if (dto == null)
            {
                throw new UserFriendlyException("Dữ liệu ghi danh không hợp lệ.", 400);
            }
            return await EnrollStudentsAsync(dto.ClassroomId, dto.StudentIds);
        }
    }
}
