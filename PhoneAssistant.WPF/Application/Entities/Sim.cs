namespace PhoneAssistant.WPF.Application.Entities;

public class Sim
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; }

    public string SimNumber { get; set; }
    
    public string? FormerUser { get; set; }

    // Navigation properties
    public virtual ICollection<Link> Links { get; set; }
}
