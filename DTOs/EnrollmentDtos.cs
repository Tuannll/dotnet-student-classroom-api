using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TH5.Dtos
{
    public class EnrollStudentsDto
    {
        [Required(ErrorMessage = "ClassroomId không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "ClassroomId không hợp lệ.")]
        public int ClassroomId { get; set; }

        [Required(ErrorMessage = "Danh sách StudentIds không được để trống.")]
        [MinLength(1, ErrorMessage = "Danh sách sinh viên ghi danh phải lớn hơn 0.")]
        public List<int> StudentIds { get; set; } = new List<int>();
    }

    public class ClassroomStudentsResponseDto
    {
        public int ClassroomId { get; set; }
        public string ClassroomName { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public List<StudentResponseDto> Students { get; set; } = new List<StudentResponseDto>();
    }

    public class StudentClassroomsResponseDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public List<ClassroomResponseDto> Classrooms { get; set; } = new List<ClassroomResponseDto>();
    }
}
