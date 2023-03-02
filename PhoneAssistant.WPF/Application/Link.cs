namespace PhoneAssistant.WPF.Features.Application;

public class Link
{
    public int Id { get; set; }
    public virtual MobilePhone? MobilePhone { get; set; }
    public int? MobilePhoneId { get; set; }
    public virtual SmartCard? SmartCard { get; set; }
    public int? SmartCardId { get; set; }
    public int? ServiceRequest { get; set; }
    public string? NewUser { get; set; }
    public string? DespatchDetails { get; set; }
    public string? Analyst { get; set; }
}