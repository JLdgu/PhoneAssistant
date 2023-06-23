namespace PhoneAssistant.WPF.Application.Entities;

public class Sim
{
    public int Id { get; set; }
    public required string PhoneNumber { get; set; }

    public required string SimNumber { get; set; }
    
    public string? FormerUser { get; set; }

    // Navigation properties
    public virtual ICollection<Link> Links { get; set; }  = null!;
}
