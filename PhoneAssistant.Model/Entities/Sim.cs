using static System.Net.Mime.MediaTypeNames;

namespace PhoneAssistant.Model;

public sealed class Sim
{
    public required string PhoneNumber { get; set; }
    public required string SIMNumber { get; set; }
    public required string BillingPeriod { get; set; }
    public required string UserName { get; set; }
    public required ulong BroadbandData { get; set; }
    public required uint TextMessages { get; set; }
    public required uint VoiceCalls { get; set; }
}
