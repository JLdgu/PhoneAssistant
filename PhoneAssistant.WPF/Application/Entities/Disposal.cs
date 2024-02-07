namespace PhoneAssistant.WPF.Application.Entities;

public class Disposal
{
    public int Id { get; set; }
    public required string Imei { get; set; }
    public string? StatusDCC { get; set; }
    public string? StatusPA { get; set; }
    public string? StatusSCC { get; set; }
    public string? Action { get; set; }
}
