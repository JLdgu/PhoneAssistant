namespace PhoneAssistant.WPF.Application.Entities;

public class Link
{
    public int Id { get; set; }
    
    // Navigation properties
    public virtual Phone? Phone { get; set; }

    public virtual Sim? Sim { get; set; }

    public virtual ServiceRequest? ServiceRequest { get; set; }
}