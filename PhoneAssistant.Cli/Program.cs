using PhoneAssistant.Cli.DisposalCommand;
using PhoneAssistant.Cli.EECommand;
using PhoneAssistant.Cli.EsimCommand;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.CommandLine;
using System.Text;

namespace PhoneAssistant.Cli;

public sealed class Program
{
    private static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiEscapeSequence.CustomTheme)
            .MinimumLevel.Debug()
            .WriteTo.File("pac.log")
            .CreateLogger();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // Register encoding provider for ExcelDataReader

        try
        {
            RootCommand rootCommand = new("Phone Assistant Command Line Interface");

            EE.Command(rootCommand);

            Esim.Command(rootCommand);

            Disposal.Command(rootCommand);

            return await rootCommand.Parse(args).InvokeAsync();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
internal static class AnsiEscapeSequence
{
    private const string Unthemed = "";
    private const string Reset = "\x1b[0m";
    private const string Bold = "\x1b[1m";

    private const string Black = "\x1b[30m";
    private const string Red = "\x1b[31m";
    private const string Green = "\x1b[32m";
    private const string Yellow = "\x1b[33m";
    private const string Blue = "\x1b[34m";
    private const string Magenta = "\x1b[35m";
    private const string Cyan = "\x1b[36m";
    private const string White = "\x1b[37m";

    private const string BrightBlack = "\x1b[30;1m";
    private const string BrightRed = "\x1b[31;1m";
    private const string BrightGreen = "\x1b[32;1m";
    private const string BrightYellow = "\x1b[33;1m";
    private const string BrightBlue = "\x1b[34;1m";
    private const string BrightMagenta = "\x1b[35;1m";
    private const string BrightCyan = "\x1b[36;1m";
    private const string BrightWhite = "\x1b[37;1m";

    private static readonly Dictionary<ConsoleThemeStyle, string> Styles = new()
    {
        [ConsoleThemeStyle.Text] = Cyan,
        [ConsoleThemeStyle.SecondaryText] = Unthemed,
        [ConsoleThemeStyle.TertiaryText] = Unthemed,
        [ConsoleThemeStyle.Invalid] = Yellow,
        [ConsoleThemeStyle.Null] = Blue,
        [ConsoleThemeStyle.Name] = Unthemed,
        [ConsoleThemeStyle.String] = Cyan,
        [ConsoleThemeStyle.Number] = Magenta,
        [ConsoleThemeStyle.Boolean] = Blue,
        [ConsoleThemeStyle.Scalar] = Green,
        [ConsoleThemeStyle.LevelVerbose] = Unthemed,
        [ConsoleThemeStyle.LevelDebug] = Bold,
        [ConsoleThemeStyle.LevelInformation] = BrightCyan,
        [ConsoleThemeStyle.LevelWarning] = BrightYellow,
        [ConsoleThemeStyle.LevelError] = BrightRed,
        [ConsoleThemeStyle.LevelFatal] = BrightRed,
    };
    internal static readonly AnsiConsoleTheme CustomTheme = new(Styles);
}
