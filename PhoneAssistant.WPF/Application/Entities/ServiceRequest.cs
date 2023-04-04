namespace PhoneAssistant.WPF.Application.Entities;

public class ServiceRequest
{
    public int Id { get; set; }
    
    public int ServiceRequestNumber { get; set; }
    
    public string NewUser { get; set; }
    
    public string? DespatchDetails { get; set; }

    // Navigation properties
    public virtual ICollection<Link> Links { get; set; }
}