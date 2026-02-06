using System.Windows.Controls;

namespace PhoneAssistant.WPF.Features.Phones;

public partial class EmailView : UserControl
{    
    public EmailView()
    {
        InitializeComponent();
        InitializeAsync();
    }
    async void InitializeAsync()
    {

        await WebView2.EnsureCoreWebView2Async(null);        
        //WebView2.CoreWebView2.Profile.PreferredColorScheme = Microsoft.Web.WebView2.Core.CoreWebView2PreferredColorScheme.Light;
        WebView2.DefaultBackgroundColor = System.Drawing.Color.LightGray;
    }
    private void DeliveryAddress_TextChanged(object sender, TextChangedEventArgs e)
    {
        string newValue = EmailViewModel.ReformatDeliveryAddress(DeliveryAddress.Text);
        if (newValue != DeliveryAddress.Text)
            DeliveryAddress.Text = newValue;
    }
}