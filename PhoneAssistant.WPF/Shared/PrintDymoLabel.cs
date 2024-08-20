using System.Drawing;
using System.Drawing.Printing;

using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.WPF.Shared
{
    public sealed class PrintDymoLabel : IPrintDymoLabel
    {
        // For Dymo 450 printer and Label 30256 Shipping w231 x h400
        // the largest rectangle we can draw is
        // graphics.DrawRectangle(_linePen, 2, 20, 365, 193);
        const int BodyHeight = 193;
        const int BodyWidth = 365;
        const int MarginTop = 20;
        const int MarginLeft = 2;

        private readonly IUserSettings _userSettings;
        private string? _address;
        private string? _includeDate;

        public PrintDymoLabel(IUserSettings userSettings)
        {
            _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
        }


        public void Execute(string address, string? includeDate)
        {
            _address = address;
            _includeDate = includeDate;

            PrintDocument pd = new();
            pd.DefaultPageSettings.Landscape = true;
            pd.DefaultPageSettings.Color = false;
            pd.DefaultPageSettings.PaperSize = new PaperSize("30256 Shipping", 231, 400);

            if (_userSettings.DymoPrintToFile)
            {
                pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                pd.DefaultPageSettings.PrinterSettings.PrintToFile = true;
                pd.DefaultPageSettings.PrinterSettings.PrintFileName = _userSettings.DymoPrintFile;
            }
            else
            {
                pd.PrinterSettings.PrinterName = _userSettings.DymoPrinter;
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
            if (_includeDate is not null)
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

            if (_includeDate is not null)
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Far;
                sf.Alignment = StringAlignment.Far;

                rectangle = new(MarginLeft, MarginTop + BodyHeight - dateFontHeight, BodyWidth, dateFontHeight);
                graphics.DrawString(_includeDate, dateFont, brush, rectangle, sf);
            }

            ev.HasMorePages = false;
        }
    }
}
