using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesItemViewModel : ObservableObject
{
    private readonly IPhonesRepository _repository;
    private readonly IPrintEnvelope _printEnvelope;
    private v1Phone _phone;

    public v1Phone Phone
    {
        get { return _phone; }
        set
        {
            _phone = value;
            PhoneNumber = _phone.PhoneNumber ?? string.Empty;
            SimNumber = _phone.SimNumber ?? string.Empty;
            if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(SimNumber))
                CanRemoveSim = false;
            else
                CanRemoveSim = true;
            if (_phone.Status == "Production")
                CanPrintEnvelope = true;
            else
                CanPrintEnvelope = false;
        }
    }

    [ObservableProperty]
    private string _phoneNumber;

    [ObservableProperty]
    private string _simNumber;

    public PhonesItemViewModel(IPhonesRepository repository, IPrintEnvelope printEnvelope)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
        
        PhoneNumber = string.Empty;
        SimNumber = string.Empty;
    }

    [RelayCommand]
    private void PrintEnvelope()
    {
        _printEnvelope.Execute(Phone);
        CanPrintEnvelope = false;
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private bool canPrintEnvelope;

    [RelayCommand]
    private void RemoveSim()
    {
        _repository.RemoveSimFromPhone(Phone);
        PhoneNumber = string.Empty;
        SimNumber = string.Empty;
        CanRemoveSim = false;
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private bool canRemoveSim;
}
