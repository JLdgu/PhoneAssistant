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

    // Expose a bubbled TextChanged routed event so parent views (like EmailView) can handle TextChanged on this control.
    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(TextChanged), RoutingStrategy.Bubble, typeof(TextChangedEventHandler), typeof(DeliveryAddressView));

    public event TextChangedEventHandler TextChanged
    {
        add => AddHandler(TextChangedEvent, value);
        remove => RemoveHandler(TextChangedEvent, value);
    }

    // Inner TextBox TextChanged handler forwards to the control's RoutedEvent.
    private void InnerDeliveryAddress_TextChanged(object? sender, TextChangedEventArgs e)
    {
        // Re-raise as this control's TextChanged routed event so parent handlers wired on the control are invoked.
        var args = new TextChangedEventArgs(TextChangedEvent, e.UndoAction);
        RaiseEvent(args);
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
}
