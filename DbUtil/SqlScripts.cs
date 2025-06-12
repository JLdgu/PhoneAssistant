using System.Collections.ObjectModel;
using System.Text;

namespace DbUtil;

public class SqlScripts
{
    public Collection<Script> Scripts { get; }

    public SqlScripts()
    {
        Scripts = [
            new Script("Drop table SchemaVersions", "DROP TABLE SchemaVersions;"),
            new Script("Restructure Locations",RestructureLocations())
        ];
    }

    public SqlScripts(Collection<Script> scripts)
    {
        Scripts = scripts;
    }

    private string RestructureLocations()
    {
        StringBuilder sb = new();
        sb.AppendLine("ALTER TABLE Locations ADD COLUMN Note TEXT;");
        sb.AppendLine("""
            UPDATE Locations SET Note = '<p><br /><span style="color: red;"><b>Important Note</b></span><br>When attending your appointment please wait in the Reception waiting room and ring 2050 then choose option 1 (Collect mobile phone).
            (01392 382050 from a mobile phone)</p>' WHERE Name = 'Collection L87';
            """);
            
        return sb.ToString();
    }
}

public record Script(string Name, string Sql);
