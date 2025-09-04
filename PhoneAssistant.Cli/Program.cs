using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.CommandLine;
using System.Text;

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
            RootCommand rootCommand = GetRootCommand();
            return rootCommand.InvokeAsync(args);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static RootCommand GetRootCommand()
    {
        RootCommand rootCommand = new("Phone Assistant Command Line Interface");

        Command baseCommand = new("base", "Import EE base report");
        Option<FileInfo> baseFileOption = new("--file") { Description = "Full path to the EE base report file to import (*.xlsx)" };
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

        StringBuilder sb = new();
        sb.AppendLine("Create a csv file containing decommissioned/disposed IMEIs that can be bulk imported to Samsung Knox.");
        sb.AppendLine();
        sb.AppendLine("The output file (knox_import.csv) will contain IMEIs that are marked as decommissioned and disposed on myScomis.");
        sb.AppendLine("Input file name expected formats are:");
        sb.AppendLine("CI List*.xlsx for myScomis import - most recent will be used");
        sb.AppendLine("kme_devices.csv");
        sb.AppendLine();
        sb.AppendLine("All files should be placed in the folder specified. Defaults to users Downloads folder");

        Command knoxCommand = new("knox", sb.ToString());
        Option<DirectoryInfo> workFolderOption = new("--folder") { Description = "Path to the folder where the output csv file should be created" };
        workFolderOption.AddAlias("-f");
        knoxCommand.AddOption(workFolderOption);

        knoxCommand.SetHandler((outputFolder) =>
        {
            try
            {
                Log.Information("Creating Knox import file");
                Knox.Execute(outputFolder);
            }
            catch (Exception ex)
            {

                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        }, workFolderOption);

        rootCommand.Add(knoxCommand);

        return rootCommand;
    }     
}
