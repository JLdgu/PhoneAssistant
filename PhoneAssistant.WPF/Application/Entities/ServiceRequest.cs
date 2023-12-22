namespace PhoneAssistant.WPF.Application.Entities;

public class ServiceRequest
{    
    public required int ServiceRequestNumber { get; set; }    
    public required string NewUser { get; set; }
    public bool? Collection { get; set; }
    public string? DespatchDetails { get; set; }
    public string LastUpdate { get; set; } = string.Empty;


    // Navigation properties
    //public virtual ICollection<Link> Links { get; set; }  = null!;
}