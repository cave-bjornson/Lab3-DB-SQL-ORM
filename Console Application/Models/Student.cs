using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication.Models;

[Table("Student")]
[Index("PersonalIdentityNumber", Name = "AK_StudentPIN", IsUnique = true)]
public partial class Student
{
    [Key]
    public int StudentId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string PersonalIdentityNumber { get; set; } = null!;

    [StringLength(25)]
    public string FirstName { get; set; } = null!;

    [StringLength(25)]
    public string LastName { get; set; } = null!;

    public int? ClassId { get; set; }

    [ForeignKey("ClassId")]
    [InverseProperty("Students")]
    public virtual Class? Class { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<StudentCourse> StudentCourses { get; } = new List<StudentCourse>();
}
