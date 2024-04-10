namespace PhoneAssistant.WPF.Application.Entities;

public class Phone
{
    public Phone()
    {
        LastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    public required string Imei { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SimNumber { get; set; }
    public string? FormerUser { get; set; }
    public required string Condition { get; set; }
    public required string Status { get; set; }
    public required OEMs OEM { get; set; }
    public required string Model { get; set; }
    public int? SR { get; set; }
    public string? AssetTag { get; set; }
    public string? NewUser { get; set; }
    public string? Notes { get; set; }

    public int? Collection { get; set; }
    public string? DespatchDetails { get; set; }
    public string LastUpdate { get; private set; }

    public void SetLastUpdate() => LastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}

public enum OEMs
{
    Apple,
    Nokia,
    Samsung,
    Other
}
