using Microsoft.EntityFrameworkCore;
using TH5.Entities;

namespace TH5.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Classroom> Classrooms { get; set; } = null!;
        public DbSet<StudentClassroom> StudentClassrooms { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.StudentCode).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.StudentCode).IsUnique();
            });

            modelBuilder.Entity<Classroom>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ClassCode).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.ClassCode).IsUnique();
            });

            modelBuilder.Entity<StudentClassroom>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Quan he voi Student
                entity.HasOne(e => e.Student)
                    .WithMany(s => s.StudentClassrooms)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Quan he voi Classroom
                entity.HasOne(e => e.Classroom)
                    .WithMany(c => c.StudentClassrooms)
                    .HasForeignKey(e => e.ClassroomId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Ràng buộc duy nhất: Một sinh viên chỉ được thêm vào một lớp 1 lần
                entity.HasIndex(e => new { e.StudentId, e.ClassroomId }).IsUnique();
            });
        }
    }
}
