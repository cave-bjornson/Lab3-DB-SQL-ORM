using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApplication.Models;

[Table("Course")]
public partial class Course
{
    [Key]
    public int CourseId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public int? TeacherId { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<StudentCourse> StudentCourses { get; } = new List<StudentCourse>();

    [ForeignKey("TeacherId")]
    [InverseProperty("Courses")]
    public virtual Employee? Teacher { get; set; }
}
