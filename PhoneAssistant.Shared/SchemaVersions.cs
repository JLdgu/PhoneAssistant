namespace PhoneAssistant.Model;
public class SchemaVersion
{
    public int SchemaVersionID { get; set; }
    public required string ScriptName { get; set; }
    public required DateTime Applied { get; set; }
}
