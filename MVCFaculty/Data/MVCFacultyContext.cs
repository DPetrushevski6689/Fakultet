using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MVCFaculty.Models
{
    public class MVCFacultyContext : IdentityDbContext<AppUser>
    {
        public MVCFacultyContext (DbContextOptions<MVCFacultyContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Enrollment>()
                .HasOne<Student>(p => p.Student)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(p => p.StudentId);

            builder.Entity<Enrollment>()
                .HasOne<Course>(p => p.Course)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(p => p.CourseId);

            builder.Entity<Course>()
                .HasOne<Teacher>(p => p.FirstTeacher)
                .WithMany(p => p.CoursesFirst)
                .HasForeignKey(p => p.FirstTeacherId);

            builder.Entity<Course>()
                .HasOne<Teacher>(p => p.SecondTeacher)
                .WithMany(p => p.CoursesSecond)
                .HasForeignKey(p => p.SecondTeacherId);
        }
    }
}
