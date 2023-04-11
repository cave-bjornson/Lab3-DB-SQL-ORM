using Spectre.Console;

namespace ConsoleApplication;

internal static partial class Program
{
    /*
     * RunEmployeeChoice
     *
     * Print a table of all employees.
     * The user can filter the table by position title.
     */
    private static void PrintEmployees()
    {
        const string positionsQuery = "SELECT P.Title FROM Position P";

        IEnumerable<string> positions = RunReaderQuery(positionsQuery)
            .Select(arr => arr.First().Item1.ToString())
            .Prepend("Everyone")!;

        var titleSelection = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Filter Position Title:").AddChoices(positions)
        );

        var queryString = """
                             SELECT E.FirstName, E.LastName, P.Title
                             FROM dbo.Employee AS E
                             INNER JOIN dbo.Position AS P
                             ON (E.PositionId = P.PositionId)
                             """;

        if (titleSelection != "Everyone")
        {
            queryString += $"WHERE P.Title = '{titleSelection}'";
        }

        var columnNames = new[] { "First Name", "Last Name", "Position" };
        PrintTable(title: "Employees", rows: RunReaderQuery(queryString), columnNames: columnNames);
    }

    /*
     * RunLastMonthGradesChoice
     *
     * Print a table of all grades from the last month.
     */
    private static void PrintLastMonthGrades()
    {
        const string queryString = """
                                   SELECT S.FirstName, S.LastName, C.Name, G.GradeValue, g.DateGraded
                                   FROM Grade G
                                   INNER JOIN StudentCourse SC on G.StudentCourseId = SC.StudentCourseId
                                   INNER JOIN Course C on SC.CourseId = C.CourseId
                                   INNER JOIN Student S on SC.StudentId = S.StudentId
                                   WHERE YEAR(DateGraded) = YEAR(GETDATE()) AND MONTH(DateGraded) = MONTH(GETDATE())
                                   """;

        IEnumerable<(object, Type)[]> rows = RunReaderQuery(queryString);
        foreach (var row in rows)
        {
            var dateValue = (DateTime)row[4].Item1;
            row[4].Item1 = DateOnly.FromDateTime(dateValue);
            row[4].Item2 = typeof(DateOnly);
        }
        var columnNames = new[] { "First Name", "Last Name", "Course", "Grade", "Date" };

        PrintTable(title: "All grades set last month", rows: rows, columnNames: columnNames);
    }

    /*
         * RunGradeStatsChoice
         *
         * Print a table of the average, max and min grade for each course.
         */
    private static void PrintGradeStats()
    {
        const string queryString = """
                                   SELECT C.Name,
                                   AVG(CAST(GradeValue AS FLOAT)) AS AvgGrade,
                                   MAX(GradeValue) AS MaxGrade,
                                   MIN(GradeValue) AS MinGrade
                                   FROM Course C
                                   JOIN StudentCourse SC on C.CourseId = SC.CourseId
                                   JOIN Grade G on SC.StudentCourseId = G.StudentCourseId
                                   GROUP BY C.Name
                                   """;
        IEnumerable<(object, Type)[]> rows = RunReaderQuery(queryString);
        foreach (var row in rows)
        {
            double doubleValue = (double)row[1].Item1;
            row[1].Item1 = Math.Round(doubleValue, 2);
        }

        var columnNames = new[] { "Course", "Average", "Max", "Min" };
        PrintTable(title: "Grade Statistics", rows: rows, columnNames: columnNames);
    }

    /*
     * RunAddStudentChoice
     *
     * Add a new student to the database with SQL queries.
     */
    private static void RunAddStudentMenu()
    {
        var (firstName, lastName, pin) = AskNamesAndPIN();
        var classDict = new Dictionary<int, string>() { [0] = "None" };
        IEnumerable<(object, Type)[]> classes = RunReaderQuery("SELECT * FROM Class");
        foreach (var myClass in classes)
        {
            var classId = (int)myClass.First().Item1;
            var className = (string)myClass.Last().Item1;
            classDict[classId] = className;
        }

        int selectedClassId = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("Select a class to enroll Student in")
                .AddChoices(classDict.Keys)
                .UseConverter(id => classDict[id])
        );

        var grid = new Grid();
        grid.AddColumns(2);
        grid.AddRow("First name:", firstName);
        grid.AddRow("Last name:", lastName);
        grid.AddRow("PIN:", pin);
        grid.AddRow("Class:", classDict[selectedClassId]);
        AnsiConsole.Write(grid);
        if (AnsiConsole.Confirm("Save this student?") == false)
        {
            return;
        }

        var queryString = string.Format(
            """
            INSERT INTO Student (FirstName, LastName, PersonalIdentityNumber, ClassId)
            VALUES (N'{0}', N'{1}', '{2}', {3})
            """,
            firstName,
            lastName,
            pin,
            selectedClassId != 0 ? selectedClassId : "NULL"
        );

        int rowsAffected = RunNonQuery(queryString);
        var resultMsg = (rowsAffected > 0) ? "Student was saved" : "Student was not saved";
        AnsiConsole.WriteLine(resultMsg);
    }
}
