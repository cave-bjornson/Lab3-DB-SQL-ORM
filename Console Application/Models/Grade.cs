using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApplication.Models;

[Table("Grade")]
public partial class Grade
{
    [Key]
    public int GradeId { get; set; }

    public int StudentCourseId { get; set; }

    public int TeacherId { get; set; }

    public byte GradeValue { get; set; }

    [Column(TypeName = "date")]
    public DateTime DateGraded { get; set; }

    [ForeignKey("StudentCourseId")]
    [InverseProperty("Grades")]
    public virtual StudentCourse StudentCourse { get; set; } = null!;

    [ForeignKey("TeacherId")]
    [InverseProperty("Grades")]
    public virtual Employee Teacher { get; set; } = null!;
}
