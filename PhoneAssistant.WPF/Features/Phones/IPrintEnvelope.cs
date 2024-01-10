using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public interface IPrintEnvelope
{
    void Execute(OrderDetails orderDetails);
}