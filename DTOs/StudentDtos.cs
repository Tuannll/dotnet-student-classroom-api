using System;
using System.ComponentModel.DataAnnotations;

namespace TH5.Dtos
{
    public class CreateStudentDto
    {
        [Required(ErrorMessage = "Tên sinh viên không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên sinh viên tối đa 50 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã số sinh viên không được để trống.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Mã số sinh viên từ 1 đến 20 ký tự.")]
        public string StudentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống.")]
        public DateOnly DateOfBirth { get; set; }
    }

    public class UpdateStudentDto
    {
        [Required(ErrorMessage = "Tên sinh viên không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên sinh viên tối đa 50 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã số sinh viên không được để trống.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Mã số sinh viên từ 1 đến 20 ký tự.")]
        public string StudentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống.")]
        public DateOnly DateOfBirth { get; set; }
    }

    public class StudentResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
    }
}
