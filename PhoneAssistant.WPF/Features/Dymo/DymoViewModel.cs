using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.WPF.Shared;
using System.Windows;

namespace PhoneAssistant.WPF.Features.Dymo;

public partial class DymoViewModel : ObservableObject, IViewModel
{
    [ObservableProperty]
    private string _label = string.Empty;
    private readonly IPrintDymoLabel _dymoLabel;

    public DymoViewModel(IPrintDymoLabel dymoLabel) => _dymoLabel = dymoLabel ?? throw new ArgumentNullException(nameof(dymoLabel));

    [RelayCommand]
    private async Task PrintDymoLabel()
    {
        await Task.Run(() =>
        {
            _dymoLabel.Execute(Label, null);
        });

        Clipboard.SetText(Label);
    }

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
