namespace PhoneAssistant.WPF.Features.Disposals;

public sealed record class LogMessage(MessageType Type, string Text, int Progress = 0);

public enum MessageType
{
    Default,
    MSMaxProgress,
    MSProgress,
    PAMaxProgress,
    PAProgress,
    SCCMaxProgress,
    SCCProgress
}