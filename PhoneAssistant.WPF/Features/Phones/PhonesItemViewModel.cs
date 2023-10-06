using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesItemViewModel : ObservableObject
{
    private readonly IPhonesRepository _repository;

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
        }
    }

    [ObservableProperty]
    private string _phoneNumber;

    [ObservableProperty]
    private string _simNumber;

    public PhonesItemViewModel(IPhonesRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [RelayCommand()]
    public void RemoveSim()
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
