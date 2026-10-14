using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TH5.Entities
{
    [Table("Classrooms")]
    public class Classroom
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string ClassCode { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxStudents { get; set; }

        public ICollection<StudentClassroom> StudentClassrooms { get; set; } = new List<StudentClassroom>();
    }
}
