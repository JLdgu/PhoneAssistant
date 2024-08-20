namespace PhoneAssistant.WPF.Shared;

public interface IPrintDymoLabel
{
    void Execute(string address, string? includeDate);
}