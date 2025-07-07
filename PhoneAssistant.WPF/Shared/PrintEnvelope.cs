using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;

using Microsoft.Extensions.FileProviders;

using PhoneAssistant.Model;

namespace PhoneAssistant.WPF.Shared;

internal sealed class PrintEnvelope : IPrintEnvelope
{
    const int A4_PAGE_HEIGHT = 1169;
    const int A4_PAGE_WIDTH = 827;
    //const int A4_BODY_HEIGHT = 969;
    const int A4_BODY_WIDTH = 667;
    const int MARGIN_TOP = 70;
    const int MARGIN_LEFT = 80;
    const int MARGIN_BOTTOM = 100;
    const int EUC_IMAGE_HEIGHT = 140;
    const int EUC_IMAGE_WIDTH = 500;
    const int IMAGE_HEIGHT = 120;
    const int IMAGE_WIDTH = 120;
    const int IMAGE_VERTICAL_PADDING = 30;

    private readonly Brush _lineBrush = new SolidBrush(Color.FromArgb(255, 91, 155, 213));
    private readonly Pen _linePen;
    private readonly Brush _blackBrush = new SolidBrush(Color.Black);
    //public Brush BlackBrush => _blackBrush;

    private readonly Font _bodyFont = new("Arial", 18);
    //private readonly Font _footerFont = new("Arial", 16);
    int _verticalPosition = MARGIN_TOP;

    private readonly IApplicationSettingsRepository _appSettings;
    private OrderDetails? _orderDetails;

    public PrintEnvelope(IApplicationSettingsRepository appSettings)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _linePen = new Pen(_lineBrush, 2);
    }

    public void Execute(OrderDetails orderDetails)
    {
        if (orderDetails is null)
        {
            throw new ArgumentNullException(nameof(orderDetails));
        }
        _orderDetails = orderDetails;
        _verticalPosition = MARGIN_TOP;

        PrintDocument pd = new();
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
        if (ev.Graphics is null || _orderDetails is null)
            return;
        Graphics graphics = ev.Graphics;

        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), "PhoneAssistant.WPF");

        DrawLine(graphics);

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
        RectangleF bodyRectangle = new(MARGIN_LEFT, _verticalPosition, A4_BODY_WIDTH, fontLineHeight * 15);

        StringFormat stringFormat = new StringFormat();
        float[] tabs = { 225 };
        stringFormat.SetTabStops(0, tabs);

        graphics.DrawString(_orderDetails.EnvelopeText, _bodyFont, _blackBrush, bodyRectangle, stringFormat);

        //_vertialPostion += (int)fontLineHeight * 15;

        _verticalPosition = A4_PAGE_HEIGHT - MARGIN_BOTTOM;
        DrawLine(graphics);

        ev.HasMorePages = false;
    }

    private void DrawLine(Graphics graphics)
    {
        graphics.DrawLine(_linePen, MARGIN_LEFT, _verticalPosition, MARGIN_LEFT + A4_BODY_WIDTH, _verticalPosition);
        _verticalPosition += (int)_linePen.Width;
    }
}
