namespace PhoneAssistant.WPF.Application.Entities;
public class ImportHistory
{
    public int Id { get; set; }
    public required ImportType Name { get; set; }
    public required string File { get; set; }
    public required string ImportDate { get; set; }
}

public enum ImportType
{
    BaseReport,
    Disposal
}