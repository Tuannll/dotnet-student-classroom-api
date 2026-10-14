using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TH5.Entities
{
    [Table("StudentClassrooms")]
    public class StudentClassroom
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int ClassroomId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student? Student { get; set; }

        [ForeignKey(nameof(ClassroomId))]
        public Classroom? Classroom { get; set; }
    }
}
