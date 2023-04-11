using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication.Models;

[Table("Employee")]
[Index("PersonalIdentityNumber", Name = "AK_EmployeePIN", IsUnique = true)]
public partial class Employee
{
    [Key]
    public int EmployeeId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string PersonalIdentityNumber { get; set; } = null!;

    [StringLength(25)]
    public string FirstName { get; set; } = null!;

    [StringLength(25)]
    public string LastName { get; set; } = null!;

    public int? PositionId { get; set; }

    [InverseProperty("Teacher")]
    public virtual ICollection<Course> Courses { get; } = new List<Course>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Grade> Grades { get; } = new List<Grade>();

    [ForeignKey("PositionId")]
    [InverseProperty("Employees")]
    public virtual Position? Position { get; set; }
}
