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
    public class ClassroomsController : ControllerBase
    {
        private readonly IClassroomService _classroomService;

        public ClassroomsController(IClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var classrooms = await _classroomService.GetAllAsync();
                return Ok(ApiResponse<List<ClassroomResponseDto>>.Success(classrooms, "Lấy danh sách lớp học thành công"));
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
                var classroom = await _classroomService.GetByIdAsync(id);
                return Ok(ApiResponse<ClassroomResponseDto>.Success(classroom, "Lấy thông tin lớp học thành công"));
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

        [HttpGet("{id:int}/students")]
        public async Task<IActionResult> GetStudents(int id)
        {
            try
            {
                var result = await _classroomService.GetStudentsInClassroomAsync(id);
                return Ok(ApiResponse<ClassroomStudentsResponseDto>.Success(result, "Lấy danh sách sinh viên của lớp học thành công"));
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
        public async Task<IActionResult> Create([FromBody] CreateClassroomDto dto)
        {
            try
            {
                var created = await _classroomService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ClassroomResponseDto>.Success(created, "Thêm lớp học thành công", 201));
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClassroomDto dto)
        {
            try
            {
                var updated = await _classroomService.UpdateAsync(id, dto);
                return Ok(ApiResponse<ClassroomResponseDto>.Success(updated, "Cập nhật lớp học thành công"));
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
                await _classroomService.DeleteAsync(id);
                return Ok(ApiResponse<object>.Success(null!, "Xóa lớp học thành công"));
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

        // Endpoint chuẩn RESTful: POST /api/classrooms/{id}/students
        [HttpPost("{id:int}/students")]
        public async Task<IActionResult> EnrollStudents(int id, [FromBody] List<int> studentIds)
        {
            try
            {
                var result = await _classroomService.EnrollStudentsAsync(id, studentIds);
                return Ok(ApiResponse<ClassroomStudentsResponseDto>.Success(result, "Ghi danh sinh viên vào lớp học thành công"));
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

        // Endpoint phụ (backward compatible): POST /api/classrooms/enroll
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollStudentsLegacy([FromBody] EnrollStudentsDto dto)
        {
            try
            {
                var result = await _classroomService.EnrollStudentsAsync(dto);
                return Ok(ApiResponse<ClassroomStudentsResponseDto>.Success(result, "Ghi danh sinh viên vào lớp học thành công"));
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
