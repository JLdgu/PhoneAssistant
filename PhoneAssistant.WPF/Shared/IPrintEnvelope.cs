namespace PhoneAssistant.WPF.Shared;

public interface IPrintEnvelope
{
    void Execute(OrderDetails orderDetails);
}