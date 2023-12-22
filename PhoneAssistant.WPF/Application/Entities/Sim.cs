namespace PhoneAssistant.WPF.Application.Entities;

public class Sim
{    
    public required string PhoneNumber { get; set; }
    public required string SimNumber { get; set; }
    public string? Status { get; set; }
    public int? SR { get; set; }
    public string? AssetTag { get; set; }
    public string? Notes { get; set; }
    public string LastUpdate { get; set; } = string.Empty;

    // Navigation properties
    //public virtual ICollection<Link> Links { get; set; } = null!;

}
