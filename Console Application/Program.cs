using Microsoft.Data.SqlClient;
using Spectre.Console;

namespace ConsoleApplication;

internal static partial class Program
{
    private static string? ConnectionString;

    static void Main(string[] args)
    {
        SqlConnectionStringBuilder builder =
            new()
            {
                InitialCatalog = "HighSchoolDB",
                MultipleActiveResultSets = true,
                Encrypt = true,
                TrustServerCertificate = true,
                ConnectTimeout = 10,
                DataSource = "."
            };
        
        // Choice disabled so we don't have to enter a password every time.
        // var choiceDict = new Dictionary<int, string>()
        // {
        //     [1] = "Windows Integrated Security",
        //     [2] = "SQL Login, for example, sa"
        // };
        //
        // var choice = AnsiConsole.Prompt(
        //     new SelectionPrompt<int>()
        //         .Title("Authenticate using:")
        //         .AddChoices(1, 2)
        //         .UseConverter(i => choiceDict[i])
        // );
        
        // Choice logic disabled so we don't have to enter a password every time.
        var choice = 2;
        if (choice == 1)
        {
            builder.IntegratedSecurity = true;
        }
        else
        {
            builder.UserID = "sa";

            AnsiConsole.WriteLine(" ");
            var password = "jiggly-hotcake";
            //var password = AnsiConsole.Prompt(new TextPrompt<string>("Enter your SQL Server password:").Secret());
            builder.Password = password;
            builder.PersistSecurityInfo = false;
        }

        ConnectionString = builder.ConnectionString;
        using (SqlConnection connection = new(ConnectionString))
        {
            AnsiConsole.WriteLine(connection.ConnectionString);

            connection.StateChange += ConnectionStateChange;
            connection.InfoMessage += ConnectionInfoMessage;
            try
            {
                AnsiConsole.WriteLine(
                    "Opening connection Please wait up to {0} seconds...",
                    builder.ConnectTimeout
                );
                connection.Open();
                AnsiConsole.WriteLine($"SQL Server version: {connection.ServerVersion}");
                connection.StatisticsEnabled = true;
            }
            catch (SqlException exception)
            {
                AnsiConsole.WriteLine($"SQL exception: {exception.Message}");
                return;
            }
        }

        // Choice of all methods in Program.SQL.MenuChoices.cs and Program.EF.MenuChoices.cs
        var choiceDict = new Dictionary<int, (string, Delegate)>()
        {
            [1] = new("Staff", PrintEmployees),
            [2] = new("Students", PrintStudents),
            [3] = new("Students by class", PrintStudentsByClass),
            [4] = new("Last month grades", PrintLastMonthGrades),
            [5] = new("Grade statistics", PrintGradeStats),
            [6] = new("Add student", RunAddStudentMenu),
            [7] = new("Add employee", RunAddEmployeeMenu),
            [8] = new(
                "Exit",
                () =>
                {
                    Environment.Exit(0);
                }
            )
        };

        var menuChoicePrompt = new SelectionPrompt<int>()
            .Title("Select a menu item:")
            .AddChoices(choiceDict.Keys)
            .UseConverter(k => choiceDict[k].Item1);

        while (true)
        {
            AnsiConsole.WriteLine("Welcome to Sunnydale High School!");
            var menuSelection = AnsiConsole.Prompt(menuChoicePrompt);
            if (menuSelection == 2)
            {
                PrintStudents();
            }
            else
            {
                choiceDict[menuSelection].Item2.DynamicInvoke();
            }
        }
    }
}
