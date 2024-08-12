using Org.BouncyCastle.Bcpg;

using PhoneAssistant.WPF.Features.Disposals;

namespace PhoneAssistant.WPF.Application.Entities;

public class Disposal
{
    public required string Imei { get; set; }
    public string StatusMS { get; set; } = ReconciliationConstants.StatusMissing;
    public string? StatusPA { get; set; }
    public int? SR { get; set; }
    public required string Manufacturer { get; set; }
    public required string Model { get; set; }
    public required bool TrackedSKU { get; set; }
    public int Certificate { get; set; }
    public string? Action { get; set; }
}
