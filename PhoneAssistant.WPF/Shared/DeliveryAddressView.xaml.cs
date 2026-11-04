using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using PhoneAssistant.Model;

namespace PhoneAssistant.WPF.Shared;

public partial class DeliveryAddressView : UserControl
{
    public DeliveryAddressView()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register(nameof(Locations), typeof(ObservableCollection<Location>), typeof(DeliveryAddressView), new PropertyMetadata(null));

    public ObservableCollection<Location>? Locations
    {
        get => (ObservableCollection<Location>?)GetValue(LocationsProperty);
        set => SetValue(LocationsProperty, value);
    }

    public static readonly DependencyProperty SelectedLocationProperty =
        DependencyProperty.Register(nameof(SelectedLocation), typeof(Location), typeof(DeliveryAddressView), new PropertyMetadata(null));

    public Location? SelectedLocation
    {
        get => (Location?)GetValue(SelectedLocationProperty);
        set => SetValue(SelectedLocationProperty, value);
    }

    public static readonly DependencyProperty DeliveryAddressProperty =
        DependencyProperty.Register(nameof(DeliveryAddress), typeof(string), typeof(DeliveryAddressView), new PropertyMetadata(string.Empty));

    public string DeliveryAddress
    {
        get => (string)GetValue(DeliveryAddressProperty);
        set => SetValue(DeliveryAddressProperty, value);
    }

    public static readonly DependencyProperty IncludeDateProperty =
        DependencyProperty.Register(nameof(IncludeDate), typeof(bool), typeof(DeliveryAddressView), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IncludeDate
    {
        get => (bool)GetValue(IncludeDateProperty);
        set => SetValue(IncludeDateProperty, value);
    }

    public static readonly DependencyProperty ShowIncludeDateProperty =
        DependencyProperty.Register(nameof(ShowIncludeDate), typeof(bool), typeof(DeliveryAddressView), new PropertyMetadata(false));

    public bool ShowIncludeDate
    {
        get => (bool)GetValue(ShowIncludeDateProperty);
        set => SetValue(ShowIncludeDateProperty, value);
    }

}
