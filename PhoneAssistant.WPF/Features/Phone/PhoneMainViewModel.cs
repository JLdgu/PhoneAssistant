using CommunityToolkit.Mvvm.ComponentModel;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Features.Phone;

internal sealed partial class PhoneMainViewModel : ObservableObject, IViewModel
{
    private readonly PhoneRepository _phoneRepository;

    public PhoneMainViewModel(PhoneRepository phoneRepository)
    {
        _phoneRepository = phoneRepository;
    }

    public ObservableCollection<Models.Phone> Phones { get; } = new();

    public async Task LoadAsync()
    {
        if (Phones.Any()) 
            return;

        var phones = await _phoneRepository.AllAsync();
        if (phones == null)
        {
            throw new ArgumentNullException(nameof(phones));
        }

        foreach (var phone in phones)
        {
            Phones.Add(phone);
        }
    }
}
