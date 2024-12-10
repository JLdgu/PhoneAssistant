using System.Collections.ObjectModel;

namespace DbUtil;

public class SqlScripts
{
    public Collection<Script> Scripts { get; }

    public SqlScripts()
    {
        Scripts = [new Script("Drop SchemaVersions", "DROP TABLE SchemaVersions;")];
    }

    public SqlScripts(Collection<Script> scripts)
    {
        Scripts = scripts;
    }
}

public record Script(string Name, string Sql);
