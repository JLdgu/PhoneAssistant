namespace PhoneAssistant.WPF.Application.Entities;

public class Location
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required bool PrintDate { get; set; }
    public string? Note { get; set; }
}