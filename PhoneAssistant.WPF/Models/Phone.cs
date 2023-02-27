namespace PhoneAssistant.WPF.Models;

public sealed class Phone
{
    public int Id { get; init; }

    public string IMEI { get; init; }

    public string? FormerUser { get; init; }

    public bool Wiped { get; init; }

    public string Status { get; init; }

    public string OEM { get; init; }

    public string? AssetTag { get; init; }

    public string? NewUser { get; init; }

    public string? DespatchDetails { get; init; }

    public string? Note { get; init; }
}
