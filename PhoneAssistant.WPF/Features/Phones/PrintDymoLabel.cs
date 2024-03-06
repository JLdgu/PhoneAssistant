using System.Drawing;
using System.Drawing.Printing;
using System.Text;

using NPOI.SS.Formula.Functions;

using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.WPF.Features.Phones
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
        private string?  _includeDate; 

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

            Brush brush = new SolidBrush(Color.Black);
            Font font = new("Segoe UI", 18);

            Rectangle rectangle = new(MarginLeft, MarginTop, BodyWidth, BodyHeight);
            graphics.DrawString(_address!, font, brush, rectangle);
            if (_includeDate is not null)
            {
                font = new("Segoe UI", 10);
                int fontHeight = (int)font.GetHeight(graphics);

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Far;
                sf.Alignment = StringAlignment.Center;

                rectangle = new(MarginLeft, (MarginTop + BodyHeight - fontHeight), BodyWidth, fontHeight);
                graphics.DrawString(_includeDate, font, brush, rectangle, sf);
            }

            ev.HasMorePages = false;
        }
    }
}
