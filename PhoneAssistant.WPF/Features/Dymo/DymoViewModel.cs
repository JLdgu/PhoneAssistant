using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;
using System.Collections.ObjectModel;
using System.Windows;

namespace PhoneAssistant.WPF.Features.Dymo;

public partial class DymoViewModel(IPrintDymoLabel dymoLabel, ILocationsRepository locationsRepository) : ViewModelBase, IViewModel
{
    private readonly IPrintDymoLabel _dymoLabel = dymoLabel ?? throw new ArgumentNullException(nameof(dymoLabel));
    private readonly ILocationsRepository _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));

    private DeliveryAddressModel? _deliveryAddressModel;
    private bool _loaded = false;

    public ObservableCollection<Location> Locations => _deliveryAddressModel?.Locations ?? new ObservableCollection<Location>();

    public Location? SelectedLocation
    {
        get => _deliveryAddressModel?.SelectedLocation;
        set { _deliveryAddressModel?.SelectedLocation = value; }
    }

    [ObservableProperty]
    public partial string Label { get; set; } = string.Empty;

    [RelayCommand]
    private async Task PrintDymoLabel()
    {
        await Task.Run(() => _dymoLabel.Execute(Label, null));
        Clipboard.SetText(Label);
    }

    public override async Task LoadAsync()
    {
        if (_loaded) return;

        if (_deliveryAddressModel is null)
        {
            _deliveryAddressModel = new DeliveryAddressModel(_locationsRepository);
            _deliveryAddressModel.SelectedLocationChanged += (s, v) => { if (v is not null) Label = v.Address; };
        }

        await _deliveryAddressModel.LoadAsync();

        _loaded = true;
    }
}
