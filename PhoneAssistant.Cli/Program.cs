using PhoneAssistant.Model;

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
            return rootCommand.Parse(args).InvokeAsync();
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
        Option<FileInfo> baseFileOption = new("--file", "-f")
        {
            Description = "Full path to the EE base report file to import (*.xlsx)",
            Required = true,
            Validators =
            {
                result =>
                {
                    var file = result.GetValueOrDefault<FileInfo>();
                    if (file == null || !file.Exists)
                    {
                        result.AddError("The specified file does not exist.");
                    }
                }
            }
        };

        baseCommand.Add(baseFileOption);
        baseCommand.SetAction(async parseResult =>
        {
            try
            {
                var baseFile = parseResult.GetValue(baseFileOption) ?? throw new ArgumentNullException(nameof(baseFileOption));
                Log.Information("Importing EE Base report");
                await BaseImport.ExecuteAsync(baseFile);
            }
            catch (Exception ex)
            {
                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        });
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
        Option<DirectoryInfo> workFolderOption = new("--folder", "-f")
        {
            Description = "Path to the folder where the output csv file should be created",
            Validators =
            {
                result =>
                {
                    var dir = result.GetValueOrDefault<DirectoryInfo>();
                    if (dir == null || !dir.Exists)
                    {
                        result.AddError("The specified folder does not exist.");
                    }
                }
            }
        };
        knoxCommand.Add(workFolderOption);

        knoxCommand.SetAction(parseResult =>
        {
            try
            {
                var outputFolder = parseResult.GetValue(workFolderOption);
                if (outputFolder is null)
                    Log.Fatal("Output folder is required");
                else
                {
                    Log.Information("Creating Knox import file");
                    Knox.Execute(outputFolder);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        });

        rootCommand.Add(knoxCommand);

        return rootCommand;
    }
}
