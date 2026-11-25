using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhoneAssistant.WPF;

public partial class SplashWindow : Window
{
    public SplashWindow(Window owner)
    {
        InitializeComponent();

        Rect ownerBounds = owner.WindowState == WindowState.Maximized && !owner.RestoreBounds.IsEmpty
            ? owner.RestoreBounds
            : new Rect(owner.Left, owner.Top, owner.Width, owner.Height);
        Left = ownerBounds.Left + (ownerBounds.Width - Width) / 2;
        Top = ownerBounds.Top + (ownerBounds.Height - Height) / 2;

#if DEBUG
        const string imagePath = "pack://application:,,,/PhoneAssistant;component/Resources/Debug.png";
#else
        const string imagePath = "pack://application:,,,/PhoneAssistant;component/Resources/Release.png";
#endif

        SplashImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
    }
}

public sealed class OutlinedTitle : FrameworkElement
{
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (ActualWidth <= 0 || ActualHeight <= 0)
            return;

        double fontSize = Math.Min(64, ActualWidth * 0.14);
        Typeface typeface = new(
            new FontFamily("Segoe UI"),
            FontStyles.Normal,
            FontWeights.Bold,
            FontStretches.Normal);
        double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
        FormattedText[] lines =
        [
            CreateText("Phone", typeface, fontSize, pixelsPerDip),
            CreateText("Assistant", typeface, fontSize, pixelsPerDip)
        ];
        Geometry[] geometries = lines.Select(line => line.BuildGeometry(new Point(0, 0))).ToArray();
        Rect[] bounds = geometries.Select(geometry => geometry.Bounds).ToArray();
        double lineGap = fontSize * 0.16;
        double totalHeight = bounds.Sum(bound => bound.Height) + lineGap;
        double top = ActualHeight / 3 - totalHeight / 2;
        Brush fill = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
        Pen outline = new(new SolidColorBrush(Color.FromArgb(190, 255, 255, 255)), 2.0);

        for (int i = 0; i < geometries.Length; i++)
        {
            double left = (ActualWidth - bounds[i].Width) / 2 - bounds[i].Left;
            double lineTop = top - bounds[i].Top;
            drawingContext.PushTransform(new TranslateTransform(left, lineTop));
            drawingContext.DrawGeometry(fill, outline, geometries[i]);
            drawingContext.Pop();
            top += bounds[i].Height + lineGap;
        }
    }

    private FormattedText CreateText(string text, Typeface typeface, double fontSize, double pixelsPerDip) =>
        new(
            text,
            System.Globalization.CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            typeface,
            fontSize,
            Brushes.Transparent,
            pixelsPerDip);
}
