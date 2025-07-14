namespace PhoneAssistant.Model;

public sealed class SimHistory
{
    public int Id { get; set; }
    public required int SimId { get; set; }
    public required int Period { get; set; }
}
