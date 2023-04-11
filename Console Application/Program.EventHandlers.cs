using System.Data;
using Microsoft.Data.SqlClient;
using Spectre.Console;

namespace ConsoleApplication;

internal static partial class Program
{
    private static void ConnectionStateChange(object sender, StateChangeEventArgs e)
    {
        AnsiConsole.MarkupLineInterpolated(
            $"[olive]State change from {e.OriginalState} to {e.CurrentState}.[/]"
        );
    }

    private static void ConnectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
        AnsiConsole.MarkupLineInterpolated($"[navy] Info: {e.Message}.[/]");
        foreach (SqlError error in e.Errors)
        {
            AnsiConsole.MarkupLineInterpolated($"[maroon]  Error: {error.Message}[/]");
        }
    }
}
