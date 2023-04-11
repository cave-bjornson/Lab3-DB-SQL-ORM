using System.Collections.Specialized;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Spectre.Console;

namespace ConsoleApplication;

internal static partial class Program
{
    /*
     * RunReaderQuery
     *
     * Run a query that returns a result set and return the result set as a list of tuples.
     * The tuples contain the value and the type of the value.
     */
    private static IEnumerable<(object, Type)[]> RunReaderQuery(string queryString)
    {
        using SqlConnection connection = new(ConnectionString);
        using SqlCommand cmd = connection.CreateCommand();

        cmd.CommandType = CommandType.Text;
        cmd.CommandText = queryString;
        connection.Open();
        using SqlDataReader reader = cmd.ExecuteReader();
        var RowTupleList = new List<(object, Type)[]>();
        foreach (IDataRecord row in reader)
        {
            var columns = new (object, Type)[row.FieldCount];
            for (int i = 0; i < row.FieldCount; i++)
            {
                columns[i] = (row.GetValue(i), row.GetFieldType(i));
            }
            RowTupleList.Add(columns);
        }
        return RowTupleList;
    }

    /*
     * RunNonQuery
     *
     * Run a query that does not return a result set and return the number of rows affected.
     */
    private static int RunNonQuery(string queryString)
    {
        using SqlConnection connection = new(ConnectionString);
        using SqlCommand cmd = connection.CreateCommand();

        cmd.CommandType = CommandType.Text;
        cmd.CommandText = queryString;
        connection.Open();
        return cmd.ExecuteNonQuery();
    }

    /*
     * PrintTable
     *
     * Print a table to the console from a list of tuples.
     */
    private static void PrintTable(
        string title,
        string[] columnNames,
        IEnumerable<(object, Type)[]> rows,
        string? footer = null
    )
    {
        var table = new Table();
        table.Title(title);
        table.AddColumns(columnNames);

        foreach (var columns in rows)
        {
            table.AddRow(
                columns
                    .Select(col => Convert.ChangeType(col.Item1, col.Item2).ToString())
                    .ToArray()!
            );
        }

        if (footer != null)
        {
            table.Caption(footer);
        }

        AnsiConsole.Write(table);
    }

    private static (string firstName, string lastName, string pin) AskNamesAndPIN()
    {
        string firstName = AnsiConsole.Ask<string>("Enter first name: ");
        string lastName = AnsiConsole.Ask<string>("Enter last name: ");
        // Does not check if the PIN is valid more than the string is 10 digits long.
        string pin = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter personal identity number: ")
                .ValidationErrorMessage("Not a valid input")
                .Validate(
                    pin =>
                        Regex.IsMatch(pin, @"^\d{10}$")
                            ? ValidationResult.Success()
                            : ValidationResult.Error("Enter 10 digits")
                )
        );
        return (firstName, lastName, pin);
    }
}
