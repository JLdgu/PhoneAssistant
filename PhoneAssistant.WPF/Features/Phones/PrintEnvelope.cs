using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;
using System.Text;

using Microsoft.Extensions.FileProviders;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;
internal sealed class PrintEnvelope : IPrintEnvelope
{
    const int A4_PAGE_HEIGHT = 1169;
    const int A4_PAGE_WIDTH = 827;
    //const int A4_BODY_HEIGHT = 969;
    const int A4_BODY_WIDTH = 627;
    const int MARGIN_TOP = 70;
    const int MARGIN_LEFT = 100;
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

    private readonly Font _bodyFont = new("Arial", 22);
    private readonly Font _footerFont = new("Arial", 10);
    int _vertialPostion = MARGIN_TOP;

    private readonly IUserSettings _userSettings;
    private v1Phone? _phone;

    public PrintEnvelope(IUserSettings userSettings)
    {
        _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
        _linePen = new Pen(_lineBrush, 2);
    }

    public void Execute(v1Phone? phone)
    {
        if (phone is null)
        {
            throw new ArgumentNullException(nameof(phone));
        }
        _phone = phone;
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
        if (ev.Graphics is null || _phone is null)
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
        RectangleF bodyRectangle = new(MARGIN_LEFT, _vertialPostion, A4_BODY_WIDTH, fontLineHeight * 9);
        string orderType = "Repurposed";
        if (_phone.NorR == "N")
            orderType = "New";
        StringBuilder bodyText = new($"Order type: {orderType}");
        bodyText.AppendLine("");
        bodyText.AppendLine("");
        bodyText.AppendLine($"Mobile Phone Type: {_phone.OEM} {_phone.Model}");
        bodyText.AppendLine("");
        bodyText.AppendLine($"Handset identifier: {_phone.Imei}");
        bodyText.AppendLine("");
        bodyText.AppendLine($"Asset Tag: {_phone.AssetTag}");
        if (!string.IsNullOrWhiteSpace(_phone.PhoneNumber))
        {
            bodyText.AppendLine("");
            bodyText.Append($"New mobile number: {_phone.PhoneNumber}");
        }
        graphics.DrawString(bodyText.ToString(), _bodyFont, _blackBrush, bodyRectangle);
        _vertialPostion += (int)fontLineHeight * 9;

        _vertialPostion = A4_PAGE_HEIGHT - MARGIN_BOTTOM;
        DrawLine(graphics);

        StringFormat alignCenter = new() { Alignment = StringAlignment.Center };
        StringFormat alignRight = new() { Alignment = StringAlignment.Far };

        fontLineHeight = _footerFont.GetHeight(graphics);
        RectangleF footerRectangle = new(MARGIN_LEFT, _vertialPostion, A4_BODY_WIDTH, fontLineHeight);
        string srText = $"Service Request# {_phone.SR}";
        graphics.DrawString(srText, _footerFont, _blackBrush, footerRectangle);

        string simText = $"SIM {_phone.SimNumber}";
        if (string.IsNullOrWhiteSpace(_phone.SimNumber))
            simText = "SIM n/a";
        graphics.DrawString(simText, _footerFont, _blackBrush, footerRectangle, alignCenter);

        string userText = $"New User {_phone.NewUser}";
        graphics.DrawString(userText, _footerFont, _blackBrush, footerRectangle, alignRight);

        ev.HasMorePages = false;
    }

    private void DrawLine(Graphics graphics)
    {
        graphics.DrawLine(_linePen, MARGIN_LEFT, _vertialPostion, MARGIN_LEFT + A4_BODY_WIDTH, _vertialPostion);
        _vertialPostion += (int)_linePen.Width;
    }
}
