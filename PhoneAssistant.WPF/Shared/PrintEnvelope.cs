using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;
using System.Text;

using Microsoft.Extensions.FileProviders;

using PhoneAssistant.WPF.Application;

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
    int _vertialPostion = MARGIN_TOP;

    private readonly IUserSettings _userSettings;
    private OrderDetails? _orderDetails;

    public PrintEnvelope(IUserSettings userSettings)
    {
        _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
        _linePen = new Pen(_lineBrush, 2);
    }

    public void Execute(OrderDetails orderDetails)
    {
        if (orderDetails is null)
        {
            throw new ArgumentNullException(nameof(orderDetails));
        }
        _orderDetails = orderDetails;
        _vertialPostion = MARGIN_TOP;

        PrintDocument pd = new();
        pd.DefaultPageSettings.Landscape = false;
        pd.DefaultPageSettings.Color = true;
        if (_userSettings.PrintToFile)
        {
            pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
            pd.DefaultPageSettings.PrinterSettings.PrintToFile = true;
            pd.DefaultPageSettings.PrinterSettings.PrintFileName = _userSettings.PrintFile;
        }
        else
        {
            pd.PrinterSettings.PrinterName = _userSettings.Printer;
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
            graphics.DrawImage(Image.FromStream(reader), A4_PAGE_WIDTH / 2 - EUC_IMAGE_WIDTH / 2, _vertialPostion, EUC_IMAGE_WIDTH, EUC_IMAGE_HEIGHT);
        }
        _vertialPostion += EUC_IMAGE_HEIGHT;

        DrawLine(graphics);

        _vertialPostion += IMAGE_VERTICAL_PADDING;
        using (var reader = embeddedProvider.GetFileInfo("Resources/Keyboard.jpg").CreateReadStream())
        {
            graphics.DrawImage(Image.FromStream(reader), 224, _vertialPostion, IMAGE_WIDTH, IMAGE_HEIGHT);
        }
        using (var reader = embeddedProvider.GetFileInfo("Resources/MobilePhone.jpg").CreateReadStream())
        {
            graphics.DrawImage(Image.FromStream(reader), 354, _vertialPostion, IMAGE_WIDTH, IMAGE_HEIGHT);
        }
        using (var reader = embeddedProvider.GetFileInfo("Resources/Monitor.png").CreateReadStream())
        {
            graphics.DrawImage(Image.FromStream(reader), 484, _vertialPostion, IMAGE_WIDTH, IMAGE_HEIGHT);
        }
        _vertialPostion += IMAGE_HEIGHT + IMAGE_VERTICAL_PADDING;

        float fontLineHeight = _bodyFont.GetHeight(graphics);
        RectangleF bodyRectangle = new(MARGIN_LEFT, _vertialPostion, A4_BODY_WIDTH, fontLineHeight * 15);

        StringBuilder bodyText = new();
        if (_orderDetails.Phone.SR > 999999)
            bodyText.AppendLine($"Issue:\t#{_orderDetails.Phone.SR}");
        else
            bodyText.AppendLine($"Service Request:\t#{_orderDetails.Phone.SR}");
        bodyText.AppendLine("");
        bodyText.AppendLine($"Device User:\t{_orderDetails.Phone.NewUser}");
        bodyText.AppendLine("");
        bodyText.AppendLine($"Order type:\t{_orderDetails.OrderedItem}");
        bodyText.AppendLine("");
        bodyText.AppendLine($"Device supplied:\t{_orderDetails.DeviceSupplied}");
        bodyText.AppendLine("");
        bodyText.AppendLine($"Handset identifier:\t{_orderDetails.Phone.Imei}");
        bodyText.AppendLine("");
        bodyText.AppendLine($"Asset Tag:\t{_orderDetails.Phone.AssetTag}");
        if (!string.IsNullOrWhiteSpace(_orderDetails.Phone.PhoneNumber))
        {
            bodyText.AppendLine("");
            bodyText.AppendLine($"Phone number:\t{_orderDetails.Phone.PhoneNumber}");
            bodyText.AppendLine("");
            bodyText.AppendLine($"SIM:\t{_orderDetails.Phone.SimNumber}");
        }
        StringFormat stringFormat = new StringFormat();
        float[] tabs = { 225 };
        stringFormat.SetTabStops(0, tabs);

        graphics.DrawString(bodyText.ToString(), _bodyFont, _blackBrush, bodyRectangle, stringFormat);

        //_vertialPostion += (int)fontLineHeight * 15;

        _vertialPostion = A4_PAGE_HEIGHT - MARGIN_BOTTOM;
        DrawLine(graphics);

        ev.HasMorePages = false;
    }

    private void DrawLine(Graphics graphics)
    {
        graphics.DrawLine(_linePen, MARGIN_LEFT, _vertialPostion, MARGIN_LEFT + A4_BODY_WIDTH, _vertialPostion);
        _vertialPostion += (int)_linePen.Width;
    }
}
