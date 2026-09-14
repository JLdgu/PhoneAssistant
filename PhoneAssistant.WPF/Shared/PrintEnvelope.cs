using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;
using System.Text;

using Microsoft.Extensions.FileProviders;

using PhoneAssistant.Model;

namespace PhoneAssistant.WPF.Shared;

internal sealed class PrintEnvelope : IPrintEnvelope
{
    const int A4_PAGE_HEIGHT = 1169;
    const int A4_PAGE_WIDTH = 827;
    const int A4_BODY_WIDTH = 667;
    const int MARGIN_TOP = 70;
    const int MARGIN_LEFT = 80;
    const int MARGIN_BOTTOM = 100;
    const int EUC_IMAGE_HEIGHT = 140;
    const int EUC_IMAGE_WIDTH = 500;
    const int IMAGE_HEIGHT = 120;
    const int IMAGE_WIDTH = 120;
    const int IMAGE_VERTICAL_PADDING = 30;

    private readonly Brush _blackBrush = new SolidBrush(Color.Black);
    private readonly Font _bodyFont = new("Arial", 18);
    private readonly Brush _lineBrush = new SolidBrush(Color.FromArgb(255, 91, 155, 213));
    private readonly Pen _linePen;
    int _verticalPosition = MARGIN_TOP;

    string _envelopeInsertText = string.Empty;

    private readonly IApplicationSettingsRepository _appSettings;

    public PrintEnvelope(IApplicationSettingsRepository appSettings)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _linePen = new Pen(_lineBrush, 2);
    }

    public void Execute(string documentName, string envelopeInsertText)
    {
        _verticalPosition = MARGIN_TOP;
        _envelopeInsertText = envelopeInsertText.Trim() ?? throw new ArgumentNullException(nameof(envelopeInsertText));

        PrintDocument pd = new()
        {
            DocumentName = documentName
        };
        pd.PrinterSettings.Copies = 1;
        pd.DefaultPageSettings.Landscape = false;
        pd.DefaultPageSettings.Color = true;
        if (_appSettings.ApplicationSettings.PrintToFile)
        {
            pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
            pd.DefaultPageSettings.PrinterSettings.PrintToFile = true;
            pd.DefaultPageSettings.PrinterSettings.PrintFileName = _appSettings.ApplicationSettings.PrintFile;
        }
        else
        {
            pd.PrinterSettings.PrinterName = _appSettings.ApplicationSettings.Printer;
        }

        pd.PrintPage += new PrintPageEventHandler(PrintPage);

        if (pd.PrinterSettings.IsValid)
            pd.Print();
    }

    void PrintPage(object sender, PrintPageEventArgs ev)
    {
        if (ev.Graphics is null)
            return;
        Graphics graphics = ev.Graphics;

        DrawLine(graphics);

        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), "PhoneAssistant.WPF");
        using (var reader = embeddedProvider.GetFileInfo("Resources/EUC.png").CreateReadStream())
        {
            graphics.DrawImage(Image.FromStream(reader), A4_PAGE_WIDTH / 2 - EUC_IMAGE_WIDTH / 2, _verticalPosition, EUC_IMAGE_WIDTH, EUC_IMAGE_HEIGHT);
        }
        _verticalPosition += EUC_IMAGE_HEIGHT;

        DrawLine(graphics);

        _verticalPosition += IMAGE_VERTICAL_PADDING;
        using (var reader = embeddedProvider.GetFileInfo("Resources/Keyboard.jpg").CreateReadStream())
        {
            graphics.DrawImage(Image.FromStream(reader), 224, _verticalPosition, IMAGE_WIDTH, IMAGE_HEIGHT);
        }
        using (var reader = embeddedProvider.GetFileInfo("Resources/MobilePhone.jpg").CreateReadStream())
        {
            graphics.DrawImage(Image.FromStream(reader), 354, _verticalPosition, IMAGE_WIDTH, IMAGE_HEIGHT);
        }
        using (var reader = embeddedProvider.GetFileInfo("Resources/Monitor.png").CreateReadStream())
        {
            graphics.DrawImage(Image.FromStream(reader), 484, _verticalPosition, IMAGE_WIDTH, IMAGE_HEIGHT);
        }
        _verticalPosition += IMAGE_HEIGHT + IMAGE_VERTICAL_PADDING;

        float fontLineHeight = _bodyFont.GetHeight(graphics);
        int lineCount = CountLines(_envelopeInsertText);
        RectangleF bodyRectangle = new(MARGIN_LEFT, _verticalPosition, A4_BODY_WIDTH, fontLineHeight * lineCount);

        StringFormat stringFormat = new();
        float[] tabs = { 225 };
        stringFormat.SetTabStops(0, tabs);

        graphics.DrawString(_envelopeInsertText, _bodyFont, _blackBrush, bodyRectangle, stringFormat);

        _verticalPosition = A4_PAGE_HEIGHT - MARGIN_BOTTOM;
        DrawLine(graphics);

        ev.HasMorePages = false;
    }

    private void DrawLine(Graphics graphics)
    {
        graphics.DrawLine(_linePen, MARGIN_LEFT, _verticalPosition, MARGIN_LEFT + A4_BODY_WIDTH, _verticalPosition);
        _verticalPosition += (int)_linePen.Width;
    }

    /// <summary>
    /// Counts the number of lines in a string by counting newline characters.
    /// Works cross-platform by checking Environment.NewLine.
    /// </summary>
    static int CountLines(string content)
    {
        if (content == null || content.Length == 0)
            return 0;

        int count = 0;
        int index = 0;

        // Count occurrences of Environment.NewLine
        while ((index = content.IndexOf(Environment.NewLine, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += Environment.NewLine.Length;
        }

        // If the last line doesn't end with a newline, count it as well
        if (!content.EndsWith(Environment.NewLine))
            count++;

        return count;
    }
}
