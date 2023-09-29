namespace PhoneAssistant.WPF.Application.Entities;

public class v1Phone
{
    public required string Imei { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SimNumber { get; set; }
    public string? FormerUser { get; set; }
    public required string NorR { get; set; }
    public required string Status { get; set; }
    public required string OEM { get; set; }
    public required string Model { get; set; }
    public int? SR { get; set; }
    public string? AssetTag { get; set; }
    public string? NewUser { get; set; }
    public string? Notes { get; set; }
    //public string? Jobtitle { get; set; }
    //public string? Department { get; set; }
    public string? LastUpdate { get; }
}
