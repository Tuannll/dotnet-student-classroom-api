using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TH5.Dtos;
using TH5.Dtos.Common;
using TH5.Exceptions;
using TH5.Services.Interfaces;

namespace TH5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var students = await _studentService.GetAllAsync();
                return Ok(ApiResponse<List<StudentResponseDto>>.Success(students, "Lấy danh sách sinh viên thành công"));
            }
            catch (UserFriendlyException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.Failure(ex.Message, ex.StatusCode));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Failure($"Đã xảy ra lỗi hệ thống: {ex.Message}", 500));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var student = await _studentService.GetByIdAsync(id);
                return Ok(ApiResponse<StudentResponseDto>.Success(student, "Lấy thông tin sinh viên thành công"));
            }
            catch (UserFriendlyException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.Failure(ex.Message, ex.StatusCode));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Failure($"Đã xảy ra lỗi hệ thống: {ex.Message}", 500));
            }
        }

        [HttpGet("{id:int}/classrooms")]
        public async Task<IActionResult> GetClassrooms(int id)
        {
            try
            {
                var result = await _studentService.GetClassroomsOfStudentAsync(id);
                return Ok(ApiResponse<StudentClassroomsResponseDto>.Success(result, "Lấy danh sách lớp học của sinh viên thành công"));
            }
            catch (UserFriendlyException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.Failure(ex.Message, ex.StatusCode));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Failure($"Đã xảy ra lỗi hệ thống: {ex.Message}", 500));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
        {
            try
            {
                var created = await _studentService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<StudentResponseDto>.Success(created, "Thêm sinh viên thành công", 201));
            }
            catch (UserFriendlyException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.Failure(ex.Message, ex.StatusCode));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Failure($"Đã xảy ra lỗi hệ thống: {ex.Message}", 500));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto)
        {
            try
            {
                var updated = await _studentService.UpdateAsync(id, dto);
                return Ok(ApiResponse<StudentResponseDto>.Success(updated, "Cập nhật sinh viên thành công"));
            }
            catch (UserFriendlyException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.Failure(ex.Message, ex.StatusCode));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Failure($"Đã xảy ra lỗi hệ thống: {ex.Message}", 500));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _studentService.DeleteAsync(id);
                return Ok(ApiResponse<object>.Success(null!, "Xóa sinh viên thành công"));
            }
            catch (UserFriendlyException ex)
            {
                return StatusCode(ex.StatusCode, ApiResponse<object>.Failure(ex.Message, ex.StatusCode));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Failure($"Đã xảy ra lỗi hệ thống: {ex.Message}", 500));
            }
        }
    }
}
