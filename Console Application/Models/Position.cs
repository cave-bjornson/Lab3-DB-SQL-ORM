using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication.Models;

[Table("Position")]
[Index("Title", Name = "UQ_Position", IsUnique = true)]
public partial class Position
{
    [Key]
    public int PositionId { get; set; }

    [StringLength(25)]
    public string Title { get; set; } = null!;

    [InverseProperty("Position")]
    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();
}
