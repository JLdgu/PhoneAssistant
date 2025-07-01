using PhoneAssistant.Model;

namespace PhoneAssistant.WPF.Features.Phones;
public interface IPhonesItemViewModelFactory
{
    PhonesItemViewModel Create(Phone phone);
}