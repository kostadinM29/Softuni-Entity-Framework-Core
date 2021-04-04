﻿using TeisterMask.Data.Models;

namespace TeisterMask.Data
{
    using Microsoft.EntityFrameworkCore;

    public class TeisterMaskContext : DbContext
    {
        public TeisterMaskContext() { }

        public TeisterMaskContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<EmployeeTask> EmployeesTasks { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeTask>()
                .HasKey(et => new
                {
                    et.EmployeeId,
                    et.TaskId
                });
            modelBuilder.Entity<EmployeeTask>(et =>
            {
                et.HasKey(et2 => new
                {
                    et2.EmployeeId,
                    et2.TaskId
                });
                et.HasOne(et => et.Employee)
                    .WithMany(t => t.EmployeesTasks)
                    .HasForeignKey(et => et.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
                et.HasOne(et => et.Task)
                    .WithMany(t => t.EmployeesTasks)
                    .HasForeignKey(et => et.TaskId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}