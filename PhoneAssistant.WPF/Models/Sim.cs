namespace PhoneAssistant.WPF.Models;

public sealed class Sim
{
    public int Id { get; init; }

    public string PhoneNumber { get; init; }

    public string SimNumber { get; init; }

    public string Status { get; init; }

    public string? Note { get; init; }
}
