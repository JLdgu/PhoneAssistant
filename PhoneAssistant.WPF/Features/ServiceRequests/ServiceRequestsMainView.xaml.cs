using System.Windows.Controls;

using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.ServiceRequests;
/// <summary>
/// Interaction logic for ServiceRequestsMainView.xaml
/// </summary>
public partial class ServiceRequestsMainView : UserControl
{
    public ServiceRequestsMainView()
    {
        InitializeComponent();
    }

    private void ServiceRequestsGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) 
        => UI_Interactions.SelectRowFromWhiteSpaceClick(e);
}
