using System.IO;
using System.Text.Json;
using System.Windows;

using PhoneAssistant.WPF.Features.MainWindow;
using Serilog;

namespace PhoneAssistant.WPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
#if DEBUG
    private const string WindowPlacementDirectory = "PhoneAssistant.Debug";
#else
    private const string WindowPlacementDirectory = "PhoneAssistant";
#endif

    private static readonly string WindowPlacementPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        WindowPlacementDirectory,
        "window-placement.json");

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        CenterOnPrimaryScreen();
        RestoreWindowPlacement();
        DataContext = viewModel;
        Loaded += async (s, e) => await viewModel.InitializeViewAsync();
        Closing += (_, _) => SaveWindowPlacement();
    }

    private void CenterOnPrimaryScreen()
    {
        Rect workArea = SystemParameters.WorkArea;
        Left = workArea.Left + (workArea.Width - Width) / 2;
        Top = workArea.Top + (workArea.Height - Height) / 2;
    }

    private void RestoreWindowPlacement()
    {
        if (!File.Exists(WindowPlacementPath))
            return;

        try
        {
            string json = File.ReadAllText(WindowPlacementPath);
            WindowPlacement? placement = JsonSerializer.Deserialize<WindowPlacement>(json);

            if (placement is null || !IsValidPlacement(placement))
                return;

            Left = placement.Left;
            Top = placement.Top;
            Width = placement.Width;
            Height = placement.Height;

            if (placement.IsMaximized)
                WindowState = WindowState.Maximized;
        }
        catch (JsonException ex)
        {
            Log.Warning(ex, "Unable to read the saved main window placement.");
        }
        catch (IOException ex)
        {
            Log.Warning(ex, "Unable to read the saved main window placement.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Log.Warning(ex, "Unable to read the saved main window placement.");
        }
    }

    private bool IsValidPlacement(WindowPlacement placement)
    {
        if (!double.IsFinite(placement.Left) ||
            !double.IsFinite(placement.Top) ||
            !double.IsFinite(placement.Width) ||
            !double.IsFinite(placement.Height) ||
            placement.Width < MinWidth ||
            placement.Height < MinHeight)
        {
            return false;
        }

        double virtualLeft = SystemParameters.VirtualScreenLeft;
        double virtualTop = SystemParameters.VirtualScreenTop;
        double virtualRight = virtualLeft + SystemParameters.VirtualScreenWidth;
        double virtualBottom = virtualTop + SystemParameters.VirtualScreenHeight;
        double visibleWidth = Math.Min(placement.Left + placement.Width, virtualRight) -
                              Math.Max(placement.Left, virtualLeft);
        double visibleHeight = Math.Min(placement.Top + placement.Height, virtualBottom) -
                               Math.Max(placement.Top, virtualTop);

        return visibleWidth >= Math.Min(100, placement.Width) &&
               visibleHeight >= Math.Min(40, placement.Height);
    }

    private void SaveWindowPlacement()
    {
        Rect bounds = WindowState == WindowState.Normal ? new Rect(Left, Top, Width, Height) : RestoreBounds;
        if (bounds.IsEmpty)
            return;

        WindowPlacement placement = new(
            bounds.Left,
            bounds.Top,
            bounds.Width,
            bounds.Height,
            WindowState == WindowState.Maximized);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(WindowPlacementPath)!);
            File.WriteAllText(WindowPlacementPath, JsonSerializer.Serialize(placement));
        }
        catch (IOException ex)
        {
            Log.Warning(ex, "Unable to save the main window placement.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Log.Warning(ex, "Unable to save the main window placement.");
        }
    }

    private sealed record WindowPlacement(double Left, double Top, double Width, double Height, bool IsMaximized);
}
