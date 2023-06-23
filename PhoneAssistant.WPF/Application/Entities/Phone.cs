namespace PhoneAssistant.WPF.Application.Entities;

public class Phone
{
    public int Id { get; set; }

    public required string Imei { get; set; }

    public string? FormerUser { get; set; }

    public bool Wiped { get; set; }

    public required string Status { get; set; }

    public required string OEM { get; set; }

    public string? AssetTag { get; set; }

    public string? Note { get; set; }

    // Navigation properties
    public virtual ICollection<Link> Links { get; set; } = null!;
}
