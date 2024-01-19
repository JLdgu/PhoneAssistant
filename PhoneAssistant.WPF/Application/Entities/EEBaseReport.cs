namespace PhoneAssistant.WPF.Application.Entities;

public class EEBaseReport
{
    public required string PhoneNumber { get; set; }
    public required string UserName { get; set; }
    public required string ContractEndDate { get; set; }
    public required string TalkPlan { get; set; }
    public required string Handset { get; set; }
    public required string SIMNumber { get; set; }
    public required string ConnectedIMEI{ get; set; }
    public required string LastUsedIMEI{ get; set; }
}
