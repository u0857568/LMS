using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team36LMSContext : DbContext
    {
        public Team36LMSContext()
        {
        }

        public Team36LMSContext(DbContextOptions<Team36LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrators> Administrators { get; set; }
        public virtual DbSet<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual DbSet<Assignments> Assignments { get; set; }
        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Enrollment> Enrollment { get; set; }
        public virtual DbSet<Professors> Professors { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Submission> Submission { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u0857568;Password=Suzhou_00;Database=Team36LMS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrators>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.UId)
                    .HasName("uID")
                    .IsUnique();

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<AssignmentCategories>(entity =>
            {
                entity.HasKey(e => e.Acid)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassID");

                entity.HasIndex(e => new { e.Name, e.ClassId })
                    .HasName("Name")
                    .IsUnique();

                entity.Property(e => e.Acid).HasColumnName("ACID");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Assignments>(entity =>
            {
                entity.HasKey(e => e.Aid)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Acid)
                    .HasName("ACID");

                entity.HasIndex(e => new { e.Name, e.Acid })
                    .HasName("Name")
                    .IsUnique();

                entity.Property(e => e.Aid)
                    .HasColumnName("AID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Acid).HasColumnName("ACID");

                entity.Property(e => e.Contents)
                    .IsRequired()
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.Due).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.Ac)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.Acid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasKey(e => e.ClassId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CourseId)
                    .HasName("CourseID");

                entity.HasIndex(e => e.UId)
                    .HasName("uID");

                entity.HasIndex(e => new { e.Year, e.Season, e.CourseId })
                    .HasName("Year")
                    .IsUnique();

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.End).HasColumnType("time");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Season)
                    .IsRequired()
                    .HasColumnType("varchar(6)");

                entity.Property(e => e.Start).HasColumnType("time");

                entity.Property(e => e.UId)
                    .IsRequired()
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_1");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_2");
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasKey(e => e.CourseId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Subject)
                    .HasName("Subject");

                entity.HasIndex(e => new { e.Number, e.Subject })
                    .HasName("Number")
                    .IsUnique();

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.Subject)
                    .HasName("PRIMARY");

                entity.Property(e => e.Subject).HasColumnType("varchar(4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.ClassId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassID");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.Grade)
                    .IsRequired()
                    .HasColumnType("varchar(2)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrollment)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrollment_ibfk_1");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Enrollment)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrollment_ibfk_2");
            });

            modelBuilder.Entity<Professors>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Subject)
                    .HasName("Subject");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Major)
                    .HasName("Major");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Major)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Major)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.Aid })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Aid)
                    .HasName("AID");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.Aid)
                    .HasColumnName("AID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Contents)
                    .IsRequired()
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.DateTime).HasColumnType("datetime");

                entity.HasOne(d => d.A)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.Aid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submission_ibfk_1");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submission_ibfk_2");
            });
        }
    }
}
