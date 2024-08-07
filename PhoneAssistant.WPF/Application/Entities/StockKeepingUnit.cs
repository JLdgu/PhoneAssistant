using NPOI.SS.Formula.Functions;

namespace PhoneAssistant.WPF.Application.Entities;
public class StockKeepingUnit
{
    public int Id { get; set; }
    public required string Manufacturer { get; set; }
    public required string Model { get; set; }
    public required bool TrackedSKU { get; set; }
}
