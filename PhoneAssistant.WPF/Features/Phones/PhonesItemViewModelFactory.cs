using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;
public sealed class PhonesItemViewModelFactory : IPhonesItemViewModelFactory
{
    public PhonesItemViewModel Create(v1Phone phone)
    {
        return new PhonesItemViewModel(phone);
    }
}
