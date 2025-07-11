using System.CommandLine;

using PhoneAssistant.Model;

using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace PhoneAssistant.Cli;

public sealed class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
            .MinimumLevel.Debug()
            .WriteTo.File("pac.log")
            .CreateLogger();

        RootCommand rootCommand = new("Phone Assistant Command Line Interface");

        Command baseCommand = new("base", "Import EE base report");

        var baseFileOption = new Option<FileInfo>("--file") { Description = "Full path to the EE base report file to import (*.xlsx)" };
        baseFileOption.AddAlias("-f");
        baseFileOption.ExistingOnly();
        baseFileOption.IsRequired = true;
        baseCommand.AddOption(baseFileOption);

        baseCommand.SetHandler(async (baseFile) =>
        {
            try
            {
                Log.Information("Importing EE Base report");
                await BaseImport.ExecuteAsync(baseFile);
            }
            catch (Exception ex)
            {

                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        }, baseFileOption);

        rootCommand.Add(baseCommand);

        try
        {
            rootCommand.InvokeAsync(args);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
