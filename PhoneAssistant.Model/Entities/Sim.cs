using static System.Net.Mime.MediaTypeNames;

namespace PhoneAssistant.Model;

public sealed class Sim
{
    public required string PhoneNumber { get; set; }
    public required string BillingPeriod { get; set; }
    public required string UserName { get; set; }
    public required string MonthlyRecurringCharge { get; set; }
    public required string OtherCosts { get; set; }
    public required int VoiceCalls { get; set; }
    public required int TextMessages { get; set; }
    public required int BroadbandData { get; set; }
}
