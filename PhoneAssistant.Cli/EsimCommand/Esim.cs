using Serilog;
using System.CommandLine;

namespace PhoneAssistant.Cli.EsimCommand;

internal static class Esim
{
    internal static void Command(RootCommand rootCommand)
    {
        Command esimCommand = new("esim", "Ensure eSim history is up to date");

        esimCommand.SetAction(parseResult =>
        {
            try
            {
                Execute();
            }
            catch (Exception ex)
            {
                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        });

        rootCommand.Add(esimCommand);
    }

    private static void Execute()
    {
        Log.Information("eSim history update started");
    }
}
