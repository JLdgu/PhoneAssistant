using System.Windows;
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
