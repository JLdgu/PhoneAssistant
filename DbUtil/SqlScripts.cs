using System.Collections.ObjectModel;

namespace DbUtil;

public class SqlScripts
{
    public Collection<Script> Scripts { get; }

    public SqlScripts()
    {
        Scripts = [
            new Script("Drop table SchemaVersions", "DROP TABLE SchemaVersions;"),
            new Script("Add SerialNumber to Phone",
                "ALTER TABLE Phones ADD COLUMN SerialNumber Text;")
                ];
    }

    public SqlScripts(Collection<Script> scripts)
    {
        Scripts = scripts;
    }
}

public record Script(string Name, string Sql);
