using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication.Models;

public partial class HighSchoolDB : DbContext
{
    public HighSchoolDB()
    {
    }

    public HighSchoolDB(DbContextOptions<HighSchoolDB> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentCourse> StudentCourses { get; set; }

    public virtual DbSet<StudentCourseView> StudentCourseViews { get; set; }

    public virtual DbSet<StudentGradeView> StudentGradeViews { get; set; }

    public virtual DbSet<TeachersView> TeachersViews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=HighSchoolDB;Integrated Security=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK_class");

            entity.Property(e => e.ClassId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course", tb => tb.HasTrigger("not_course_teacher"));

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses).HasConstraintName("FK_Course_Teacher");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.PersonalIdentityNumber).IsFixedLength();

            entity.HasOne(d => d.Position).WithMany(p => p.Employees).HasConstraintName("FK_Employee_Position");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable("Grade", tb =>
                {
                    tb.HasTrigger("mod_grade_value");
                    tb.HasTrigger("not_grade_teacher");
                });

            entity.Property(e => e.DateGraded).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.StudentCourse).WithMany(p => p.Grades)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grade_StudentCourse");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Grades)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Grade_Teacher");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.Property(e => e.PositionId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK_student");

            entity.Property(e => e.PersonalIdentityNumber).IsFixedLength();

            entity.HasOne(d => d.Class).WithMany(p => p.Students).HasConstraintName("FK_Student_Class");
        });

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(e => e.StudentCourseId).HasName("PK_student_course");

            entity.HasOne(d => d.Course).WithMany(p => p.StudentCourses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentCourse_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentCourses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentCourse_Student");
        });

        modelBuilder.Entity<StudentCourseView>(entity =>
        {
            entity.ToView("StudentCourseView");
        });

        modelBuilder.Entity<StudentGradeView>(entity =>
        {
            entity.ToView("StudentGradeView");
        });

        modelBuilder.Entity<TeachersView>(entity =>
        {
            entity.ToView("TeachersView");

            entity.Property(e => e.PersonalIdentityNumber).IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
