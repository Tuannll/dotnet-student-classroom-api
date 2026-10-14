using System.ComponentModel.DataAnnotations;

namespace TH5.Dtos
{
    public class CreateClassroomDto
    {
        [Required(ErrorMessage = "Tên lớp không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên lớp tối đa 50 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã lớp không được để trống.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Mã lớp từ 5 đến 20 ký tự.")]
        public string ClassCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số lượng sinh viên tối đa không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng sinh viên tối đa phải lớn hơn 0.")]
        public int MaxStudents { get; set; }
    }

    public class UpdateClassroomDto
    {
        [Required(ErrorMessage = "Tên lớp không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên lớp tối đa 50 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã lớp không được để trống.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Mã lớp từ 5 đến 20 ký tự.")]
        public string ClassCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số lượng sinh viên tối đa không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng sinh viên tối đa phải lớn hơn 0.")]
        public int MaxStudents { get; set; }
    }

    public class ClassroomResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public int CurrentStudentCount { get; set; }
    }
}
