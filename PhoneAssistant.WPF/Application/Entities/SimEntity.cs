namespace PhoneAssistant.WPF.Application.Entities;

public class SimEntity
{
    public int Id { get; set; }
    public string SimNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string? FormerUser { get; set; }
    public virtual StateEntity State { get; set; }

    public virtual ServiceRequestEntity? Link { get; set; }
}
