using PhoneAssistant.Model;
using System.Drawing;
using System.Drawing.Printing;

namespace PhoneAssistant.WPF.Shared;

public interface IPrintDymoLabel
{
    void Execute(string address, bool includeDate);
}

public sealed class PrintDymoLabel(IApplicationSettingsRepository appSettings) : IPrintDymoLabel
{
    // For Dymo 450 printer and Label 30256 Shipping w231 x h400
    // the largest rectangle we can draw is
    // graphics.DrawRectangle(_linePen, 2, 20, 365, 193);
    const int BodyHeight = 193;
    const int BodyWidth = 365;
    const int MarginTop = 20;
    const int MarginLeft = 2;

    private readonly IApplicationSettingsRepository _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    private string? _address;
    private bool _includeDate;

    public void Execute(string address, bool includeDate)
    {
        _address = address.Trim();
        _includeDate = includeDate;

        PrintDocument pd = new();
        pd.DefaultPageSettings.Landscape = true;
        pd.DefaultPageSettings.Color = false;
        pd.DefaultPageSettings.PaperSize = new PaperSize("30256 Shipping", 231, 400);

        if (_appSettings.ApplicationSettings.DymoPrintToFile)
        {
            pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
            pd.DefaultPageSettings.PrinterSettings.PrintToFile = true;
            pd.DefaultPageSettings.PrinterSettings.PrintFileName = _appSettings.ApplicationSettings.DymoPrintFile;
        }
        else
        {
            pd.PrinterSettings.PrinterName = _appSettings.ApplicationSettings.DymoPrinter;
            pd.DefaultPageSettings.PrinterSettings.PrintToFile = false;
        }

        pd.PrintPage += new PrintPageEventHandler(PrintPage);

        if (pd.PrinterSettings.IsValid)
            pd.Print();
    }

    private void PrintPage(object sender, PrintPageEventArgs ev)
    {
        if (ev.Graphics is null) return;
        Graphics graphics = ev.Graphics;

        Font dateFont = new("Segoe UI", 10);
        int dateFontHeight = (int)dateFont.GetHeight(graphics);

        int maxHeight = BodyHeight;
        if (_includeDate)
            maxHeight -= dateFontHeight;

        float fontSize = 22;
        Font font = new("Segoe UI", fontSize);
        while (true)
        {
            SizeF stringSize = new SizeF();
            stringSize = ev.Graphics.MeasureString(_address, font, BodyWidth);

            if (stringSize.Height <= maxHeight)
                break;

            fontSize = (float)(fontSize - 0.5);
            font = new("Segoe UI", fontSize);
        }

        Brush brush = new SolidBrush(Color.Black);
        Rectangle rectangle = new(MarginLeft, MarginTop, BodyWidth, maxHeight);
        graphics.DrawString(_address, font, brush, rectangle);

        if (_includeDate)
        {
            StringFormat sf = new()
            {
                LineAlignment = StringAlignment.Far,
                Alignment = StringAlignment.Far
            };

            rectangle = new(MarginLeft, MarginTop + BodyHeight - dateFontHeight, BodyWidth, dateFontHeight);
            graphics.DrawString(Utility.ToOrdinalWorkingDate(DateTime.Now, true), dateFont, brush, rectangle, sf);
        }

        ev.HasMorePages = false;
    }

    public static string ToOrdinalWorkingDate(DateTime date, bool hexSuperscript = false, int buffer = 0)
    {
        DateTime weekDay = date.AddDays(buffer);
        if (buffer > 0)
        {
            while (weekDay.DayOfWeek == DayOfWeek.Saturday || weekDay.DayOfWeek == DayOfWeek.Sunday)
                weekDay = weekDay.AddDays(buffer);
        }

        string ordinalDay = string.Empty;
        int number = weekDay.Day;
        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                ordinalDay = hexSuperscript ? number.ToString() + "\x1D57\x02B0" : number.ToString() + "<sup>th</sup>";
                break;
        }

        if (ordinalDay == string.Empty)
        {
            switch (number % 10)
            {
                case 1:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x02E2\x1D57" : number.ToString() + "<sup>st</sup>";
                    break;
                case 2:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x207F\x1D48" : number.ToString() + "<sup>nd</sup>";
                    break;
                case 3:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x02B3\x1D48" : number.ToString() + "<sup>rd</sup>";
                    break;
                default:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x1D57\x02B0" : number.ToString() + "<sup>th</sup>";
                    break;
            }
        }
        string from = weekDay.ToString("dddd * MMMM yyyy");
        from = from.Replace("*", ordinalDay);

        return from;
    }
}
