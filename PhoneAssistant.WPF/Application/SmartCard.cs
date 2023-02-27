namespace PhoneAssistant.WPF.Features.Application;

public class SmartCard
{
    public int Id { get; set; }
    public string SimNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string? FormerUser { get; set; }
    public virtual StateDTO State { get; set; }    

    public virtual Link? Link { get; set; }
}
