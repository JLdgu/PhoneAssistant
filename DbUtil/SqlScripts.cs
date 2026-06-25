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
            new Script("Restructure Locations",RestructureLocations),
            new Script("Add 3 columns to Phones", Add3ColumnsToPhones),
            new Script("Create SIMs Table", CreateSIMsTable),
            new Script("Add eSIM to SIMs Table", "ALTER TABLE Sims ADD COLUMN eSIM INTEGER;")
        ];
    }

    public SqlScripts(Collection<Script> scripts)
    {
        Scripts = scripts;
    }
    private static string Add3ColumnsToPhones
    {
        get
        {
            StringBuilder sb = new();
            sb.AppendLine("ALTER TABLE Phones ADD COLUMN SerialNumber TEXT;");
            sb.AppendLine("ALTER TABLE Phones ADD COLUMN eSIM INTEGER;");
            sb.AppendLine("ALTER TABLE Phones ADD COLUMN IncludeOnTrackingSheet INTEGER;");

            return sb.ToString();
        }
    }

    private const string CreateSIMsTable = """
        CREATE TABLE Sims (
            PhoneNumber             TEXT NOT NULL,
           	BillingPeriod           TEXT NOT NULL,
            SIMNumber               TEXT NOT NULL,
            UserName                TEXT NOT NULL,
            VoiceCalls              INTEGER NOT NULL,
            TextMessages            INTEGER NOT NULL,
            BroadbandData           INTEGER NOT NULL,
        	PRIMARY KEY (PhoneNumber, BillingPeriod)        
        );
        """;

    private static string RestructureLocations
    {
        get
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
}

public record Script(string Name, string Sql);
