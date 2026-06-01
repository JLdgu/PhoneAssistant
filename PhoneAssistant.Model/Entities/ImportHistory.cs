namespace PhoneAssistant.Model;

public class ImportHistory
{
    public required ImportType Name { get; set; }
    public required string Run { get; set; }
    public required string ImportDate { get; set; }
}

public enum ImportType
{
    BaseReport,
    Reconiliation
}