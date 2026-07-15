using CommunityToolkit.Mvvm.ComponentModel;
using PhoneAssistant.Model;
using System.Collections.ObjectModel;

namespace PhoneAssistant.WPF.Shared;

public partial class DeliveryAddressModel(ILocationsRepository locationsRepository) : ObservableObject
{
    private readonly ILocationsRepository _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
    private bool _loaded = false;

    public ObservableCollection<Location> Locations { get; } = [];

    [ObservableProperty]
    public partial Location? SelectedLocation { get; set; }

    public event EventHandler<Location?>? SelectedLocationChanged;

    partial void OnSelectedLocationChanged(Location? value)
    {
        SelectedLocationChanged?.Invoke(this, value);
    }

    public async Task LoadAsync()
    {
        if (_loaded) return;

        IEnumerable<Location> locations = await _locationsRepository.GetAllLocationsAsync();
        foreach (var location in locations)
            Locations.Add(location);

        _loaded = true;
    }
}
