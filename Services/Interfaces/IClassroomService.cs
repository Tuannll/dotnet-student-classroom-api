using System.Collections.Generic;
using System.Threading.Tasks;
using TH5.Dtos;

namespace TH5.Services.Interfaces
{
    public interface IClassroomService
    {
        Task<List<ClassroomResponseDto>> GetAllAsync();
        Task<ClassroomResponseDto> GetByIdAsync(int id);
        Task<ClassroomResponseDto> CreateAsync(CreateClassroomDto dto);
        Task<ClassroomResponseDto> UpdateAsync(int id, UpdateClassroomDto dto);
        Task DeleteAsync(int id);
        Task<ClassroomStudentsResponseDto> GetStudentsInClassroomAsync(int classroomId);
        Task<ClassroomStudentsResponseDto> EnrollStudentsAsync(int classroomId, List<int> studentIds);
        Task<ClassroomStudentsResponseDto> EnrollStudentsAsync(EnrollStudentsDto dto);
    }
}
