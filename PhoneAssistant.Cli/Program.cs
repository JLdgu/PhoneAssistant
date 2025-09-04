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
        sb.AppendLine("Create a csv file that can be bulk imported to Samsung Knox");
        sb.AppendLine("the output file will contain IMEIs that are marked as decommissioned and disposed on myScomis");
        Command knoxCommand = new("knox", sb.ToString());
        Option<FileInfo> ciFileOption = new("--ci") { Description = "Full path an xlsx file containing a list of myScomis CIs" };
        ciFileOption.ExistingOnly();
        ciFileOption.IsRequired = true;
        knoxCommand.AddOption(ciFileOption);
        Option<FileInfo> knoxFileOption = new("--knox") { Description = "Full path to a csv file containing a list of managed IMEIs exported from Samsung Knox" };
        knoxFileOption.ExistingOnly();
        knoxFileOption.IsRequired = true;
        knoxCommand.AddOption(knoxFileOption);
        Option<DirectoryInfo> outputFolderOption = new("--output") { Description = "Path to the folder where the output csv file should be created" };
        outputFolderOption.AddAlias("-o");
        outputFolderOption.ExistingOnly();
        outputFolderOption.IsRequired = true;
        knoxCommand.AddOption(outputFolderOption);

        knoxCommand.SetHandler((ciFile, knoxFile, outputFolder) =>
        {
            try
            {
                Log.Information("Creating Knox import file");
                Knox.Execute(ciFile, knoxFile, outputFolder);
            }
            catch (Exception ex)
            {

                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        }, ciFileOption, knoxFileOption, outputFolderOption);

        rootCommand.Add(knoxCommand);

        return rootCommand;
    }     
}
