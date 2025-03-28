using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebUniversity.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Account { get; set; }

    public virtual DbSet<Class> Class { get; set; }

    public virtual DbSet<ClassSchedule> ClassSchedule { get; set; }

    public virtual DbSet<ClassShift> ClassShift { get; set; }

    public virtual DbSet<Course> Course { get; set; }

    public virtual DbSet<Faculty> Faculty { get; set; }

    public virtual DbSet<Lecturer> Lecturer { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<RoleGroup> RoleGroup { get; set; }

    public virtual DbSet<RoleInRoleGroup> RoleInRoleGroup { get; set; }

    public virtual DbSet<Room> Room { get; set; }

    public virtual DbSet<Student> Student { get; set; }

    public virtual DbSet<Subject> Subject { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC074F5D5C45");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E4F81D9A31").IsUnique();

            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.LecturerId)
                .HasConstraintName("FK__Account__Lecture__73BA3083");

            entity.HasOne(d => d.RoleGroup).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_RoleGroup");

            entity.HasOne(d => d.Student).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Account__Student__72C60C4A");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Class__3214EC078B23491F");

            entity.ToTable("Class");

            entity.Property(e => e.ClassName).HasMaxLength(100);
            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Faculty).WithMany(p => p.Classes)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK__Class__FacultyId__4222D4EF");
        });

        modelBuilder.Entity<ClassSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassSch__3214EC07BE111558");

            entity.ToTable("ClassSchedule", tb =>
                {
                    tb.HasTrigger("TR_ClassSchedule_Unique");
                    tb.HasTrigger("TR_Update_ClassSchedule_Unique");
                });

            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Class).WithMany(p => p.ClassSchedules)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassSchedule_Class");

            entity.HasOne(d => d.ClassShift).WithMany(p => p.ClassSchedules)
                .HasForeignKey(d => d.ClassShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassSchedule_ClassShift");

            entity.HasOne(d => d.Course).WithMany(p => p.ClassSchedules)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassSchedule_Course");

            entity.HasOne(d => d.Room).WithMany(p => p.ClassSchedules)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassSchedule_Room");
        });

        modelBuilder.Entity<ClassShift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassShi__3214EC07341B22F7");

            entity.ToTable("ClassShift");

            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC07372680A6");

            entity.ToTable("Course");

            entity.Property(e => e.CourseName).HasMaxLength(50);
            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Courses)
                .HasForeignKey(d => d.LecturerId)
                .HasConstraintName("FK__Course__Lecturer__5812160E");

            entity.HasOne(d => d.Subject).WithMany(p => p.Courses)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK__Course__SubjectI__571DF1D5");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Faculty__3214EC078EC48E7C");

            entity.ToTable("Faculty");

            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FacultyName).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lecturer__3214EC07A43C83A4");

            entity.ToTable("Lecturer", tb => tb.HasTrigger("trg_LecturerCode"));

            entity.HasIndex(e => e.Email, "UQ__Lecturer__A9D105346CE22659").IsUnique();

            entity.HasIndex(e => e.LecturerCode, "UQ__Lecturer__BB9CDAB39FAEF2BE").IsUnique();

            entity.Property(e => e.Cccd).HasMaxLength(13);
            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.LecturerCode).HasMaxLength(20);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Faculty).WithMany(p => p.Lecturers)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK__Lecturer__Facult__4D94879B");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<RoleGroup>(entity =>
        {
            entity.ToTable("RoleGroup");

            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<RoleInRoleGroup>(entity =>
        {
            entity.ToTable("RoleInRoleGroup");

            entity.HasOne(d => d.RoleGroup).WithMany(p => p.RoleInRoleGroups)
                .HasForeignKey(d => d.RoleGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleInRoleGroup_RoleGroup");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleInRoleGroups)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleInRoleGroup_Role");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("Room");

            entity.Property(e => e.Building).HasMaxLength(50);
            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Floor).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Vacuity).HasDefaultValue(true);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Student__3214EC077783B928");

            entity.ToTable("Student", tb => tb.HasTrigger("trg_StudentCode"));

            entity.HasIndex(e => e.StudentCode, "UQ__Student__1FC886046879061C").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Student__A9D10534E0C6A5FE").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Cccd)
                .HasMaxLength(13)
                .HasColumnName("CCCD");
            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.StudentCode).HasMaxLength(20);

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Student__ClassId__47DBAE45");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subject__3214EC0729CA485C");

            entity.ToTable("Subject");

            entity.Property(e => e.CreateDay)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.SubjectName).HasMaxLength(100);

            entity.HasOne(d => d.Faculty).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK__Subject__Faculty__52593CB8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
