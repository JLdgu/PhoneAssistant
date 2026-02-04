using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.WPF.Shared;
using System.Windows;

namespace PhoneAssistant.WPF.Features.Dymo;

public partial class DymoViewModel(IPrintDymoLabel dymoLabel) : ViewModelBase, IViewModel
{
    [ObservableProperty]
    private string _label = string.Empty;
    private readonly IPrintDymoLabel _dymoLabel = dymoLabel ?? throw new ArgumentNullException(nameof(dymoLabel));

    [RelayCommand]
    private async Task PrintDymoLabel()
    {
        await Task.Run(() => _dymoLabel.Execute(Label, null));

        Clipboard.SetText(Label);
    }
}
