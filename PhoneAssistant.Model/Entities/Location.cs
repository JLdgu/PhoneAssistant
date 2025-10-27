namespace PhoneAssistant.Model;

public class Location
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required bool Collection { get; set; }
    public string? Note { get; set; }
}