using System.Linq.Expressions;
using ConsoleApplication.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace ConsoleApplication;

internal static partial class Program
{
    /*
     * PrintStudents
     *
     * Print a table of all students.
     * The user can sort the table by first name or last name.
     * The user can filter the table by class.
     */
    private static void PrintStudents(string? byClass = null)
    {
        var sortName = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Sort by:").AddChoices("First Name", "Last Name")
        );

        var sortDirection = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Sort order:").AddChoices("Ascending", "Descending")
        );

        var sortNameSelector = sortName switch
        {
            "First Name" => (Expression<Func<Student, string>>)(s => s.FirstName),
            "Last Name" => s => s.LastName,
            _ => throw new ArgumentOutOfRangeException(nameof(sortName), sortName, null)
        };

        using HighSchoolDB db = new();

        IQueryable<Student> studentsQuery = db.Students.Include(student => student.Class);
        if (byClass != null)
        {
            studentsQuery = studentsQuery.Where(student => student.Class.ClassName == byClass);
        }

        studentsQuery = sortDirection switch
        {
            "Ascending" => studentsQuery.OrderBy(sortNameSelector),
            "Descending" => studentsQuery.OrderByDescending(sortNameSelector),
            _ => throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null)
        };

        // Convert the students to a list of tuples of (object, Type)
        // to use the same PrintTable method as in SQL version.
        var studentRows = new List<(object, Type)[]>();
        foreach (var student in studentsQuery)
        {
            studentRows.Add(
                new (object, Type)[]
                {
                    (student.FirstName, typeof(string)),
                    (student.LastName, typeof(string)),
                    (student.Class?.ClassName ?? "No Class", typeof(string))
                }
            );
        }

        string className = byClass ?? "all Classes";

        PrintTable(
            $"Students in {className}",
            new[] { "First Name", "Last Name", "Class" },
            studentRows,
            $"Sorted by {sortName}, {sortDirection} order"
        );
    }

    /*
     * PrintStudentsByClass
     *
     * Print a table of all students.
     * Uses PrintStudents to print the table.
     */
    private static void PrintStudentsByClass()
    {
        using HighSchoolDB db = new();
        var classes = db.Classes.Select(c => c.ClassName).Distinct();
        var classSelection = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Filter by Class:").AddChoices(classes)
        );
        PrintStudents(classSelection);
    }

    private static void RunAddEmployeeMenu()
    {
        var (firstName, lastName, pin) = AskNamesAndPIN();

        // Choose position for new employee
        using HighSchoolDB db = new();
        var positions = db.Positions;
        var positionSelection = AnsiConsole.Prompt(
            new SelectionPrompt<Position>()
                .Title("Position:")
                .AddChoices(positions)
                .UseConverter(p => p.Title)
        );

        var grid = new Grid();
        grid.AddColumns(2);
        grid.AddRow("First name:", firstName);
        grid.AddRow("Last name:", lastName);
        grid.AddRow("PIN:", pin);
        grid.AddRow("Position:", positionSelection.Title);
        AnsiConsole.Write(grid);
        if (AnsiConsole.Confirm("Save this employee?") == false)
        {
            return;
        }

        var employee = new Employee()
        {
            FirstName = firstName,
            LastName = lastName,
            PersonalIdentityNumber = pin,
            Position = positionSelection
        };

        db.Employees.Add(employee);
        db.SaveChanges();
    }
}
