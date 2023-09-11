using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public interface IPrintEnvelope
{
    void Execute(v1Phone? phone);
}