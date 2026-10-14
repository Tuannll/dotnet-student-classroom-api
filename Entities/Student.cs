using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TH5.Entities
{
    [Table("Students")]
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string StudentCode { get; set; } = string.Empty;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        public ICollection<StudentClassroom> StudentClassrooms { get; set; } = new List<StudentClassroom>();
    }
}
