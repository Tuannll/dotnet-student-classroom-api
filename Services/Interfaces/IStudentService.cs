using System.Collections.Generic;
using System.Threading.Tasks;
using TH5.Dtos;

namespace TH5.Services.Interfaces
{
    public interface IStudentService
    {
        Task<List<StudentResponseDto>> GetAllAsync();
        Task<StudentResponseDto> GetByIdAsync(int id);
        Task<StudentResponseDto> CreateAsync(CreateStudentDto dto);
        Task<StudentResponseDto> UpdateAsync(int id, UpdateStudentDto dto);
        Task DeleteAsync(int id);
        Task<StudentClassroomsResponseDto> GetClassroomsOfStudentAsync(int studentId);
    }
}
