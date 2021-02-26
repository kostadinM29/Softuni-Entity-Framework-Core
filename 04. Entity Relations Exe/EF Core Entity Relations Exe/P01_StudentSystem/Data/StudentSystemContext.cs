using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
    public partial class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
        }

        public StudentSystemContext(DbContextOptions<StudentSystemContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=SoftUni;Integrated Security=True");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(e =>
            {
                e.Property(s => s.PhoneNumber).IsRequired(false);
            });

            modelBuilder.Entity<Course>(e =>
            {
                e.Property(c => c.Description).IsUnicode().IsRequired(false);
            });

            modelBuilder.Entity<Resource>(e =>
            {

                e.Property(r => r.Url).IsUnicode(false);

            });

            modelBuilder.Entity<Homework>(e =>
            {
                e.Property(h => h.Content).IsUnicode(false);
            });

            modelBuilder.Entity<StudentCourse>(e =>
            {
                e.HasKey(sc => new
                {
                    sc.StudentId,
                    sc.CourseId
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
