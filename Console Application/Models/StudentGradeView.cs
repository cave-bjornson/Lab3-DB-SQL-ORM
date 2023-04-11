using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication.Models;

[Keyless]
public partial class StudentGradeView
{
    public int StudentCourseId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(25)]
    public string FirstName { get; set; } = null!;

    [StringLength(25)]
    public string LastName { get; set; } = null!;

    public byte GradeValue { get; set; }

    public int TeacherId { get; set; }

    [StringLength(25)]
    public string Expr1 { get; set; } = null!;

    [StringLength(25)]
    public string Expr2 { get; set; } = null!;
}
