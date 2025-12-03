using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.CommandLine;

namespace PhoneAssistant.Cli;

public sealed class Program
{
    private static Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
            .MinimumLevel.Debug()
            .WriteTo.File("pac.log")
            .CreateLogger();
        try
        {
            RootCommand rootCommand = new("Phone Assistant Command Line Interface");

            Base.Command(rootCommand);

            Disposal.Command(rootCommand);

            Knox.Command(rootCommand);

            return rootCommand.Parse(args).InvokeAsync();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
