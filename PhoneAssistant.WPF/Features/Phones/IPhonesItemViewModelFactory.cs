using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;
public interface IPhonesItemViewModelFactory
{
    PhonesItemViewModel Create(Phone phone);
}