using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication.Models;

[Keyless]
public partial class TeachersView
{
    public int EmployeeId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string PersonalIdentityNumber { get; set; } = null!;

    [StringLength(25)]
    public string FirstName { get; set; } = null!;

    [StringLength(25)]
    public string LastName { get; set; } = null!;

    [StringLength(25)]
    public string Title { get; set; } = null!;
}
