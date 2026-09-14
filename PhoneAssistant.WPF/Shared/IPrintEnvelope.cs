namespace PhoneAssistant.WPF.Shared;

public interface IPrintEnvelope
{
    void Execute(string documentName, string envelopeInsertText);
}