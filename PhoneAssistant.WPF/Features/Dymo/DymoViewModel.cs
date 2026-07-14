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

    private bool _loaded = false;
    public ObservableCollection<Location> Locations { get; set; } = [];

    [ObservableProperty]
    public partial Location? SelectedLocation { get; set; }

    partial void OnSelectedLocationChanged(Location? value)
    {
        if (value is null) return;

        Label = value.Address;
    }

    [ObservableProperty]
    public partial string Label { get; set;} = string.Empty;

    [RelayCommand]
    private async Task PrintDymoLabel()
    {
        await Task.Run(() => _dymoLabel.Execute(Label, null));
        Clipboard.SetText(Label);
    }

    public override async Task LoadAsync()
    {
        if (_loaded) return;

        IEnumerable<Location> locations = await _locationsRepository.GetAllLocationsAsync();
        foreach (var location in locations)
            Locations.Add(location);

        _loaded = true;
    }
}
