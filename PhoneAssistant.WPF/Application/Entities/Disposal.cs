namespace PhoneAssistant.WPF.Application.Entities;

public class Disposal
{
    public required string Imei { get; set; }
    public string? StatusMS { get; set; }
    public string? StatusPA { get; set; }
    public string? StatusSCC { get; set; }
    public int? SR { get; set; }
    public int? Certificate { get; set; }
    public string? Action { get; set; }
}
