using FluentAssertions;

using FluentResults;

using Microsoft.Data.Sqlite;
using Xunit.Abstractions;

namespace DbUtil.Tests;

public class ProgramTests(ITestOutputHelper output)
{
    [Fact]
    public void ApplyScript_ShouldApplyScript_WhenScriptNotAlreadyApplied()
    {
        using SqliteConnection connection = CreateAndOpenSqliteConnection();
        CreateSchemaVersionsTable(connection);
        var script = new Script("script1", "CREATE TABLE test (Name TEXT);");

        Result<bool> scriptApplied = Program.ApplyScript(connection, script);

        scriptApplied.Value.Should().BeTrue();
    }

    [Fact]
    public void ApplyScript_ShouldNotApplyScript_WhenScriptAlreadyApplied()
    {
        using SqliteConnection connection = CreateAndOpenSqliteConnection();
        CreateSchemaVersionsTable(connection);
        var script = new Script("script1", "some sql");
        var command = connection.CreateCommand();
        command.CommandText = $"INSERT INTO SchemaVersion (ScriptName) VALUES ('{script.Name}');";
        command.ExecuteNonQuery();

        Result<bool> scriptApplied = Program.ApplyScript(connection,script);

        scriptApplied.Value.Should().BeFalse();
    }

    [Fact]
    public void ApplyScript_ShouldReturnError_WhenScriptInvalid()
    {
        using SqliteConnection connection = CreateAndOpenSqliteConnection();
        CreateSchemaVersionsTable(connection);
        var script = new Script("script1", "CREATE TABLE (Name TEXT);");

        Result<bool> scriptApplied = Program.ApplyScript(connection, script);

        scriptApplied.IsFailed.Should().BeTrue();
        scriptApplied.Reasons.Select(reason => reason.Message).First().Should().StartWith("SQLite Error 1:");
    }

    [Fact]
    public void CheckSchemaVersionsExists_ShouldCreateSchemaVersionTable_WhenTableDoesNotExist()
    {
        using SqliteConnection connection = CreateAndOpenSqliteConnection();
        
        bool tableCreated = Program.CheckSchemaVersionsExists(connection);

        tableCreated.Should().BeTrue();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT count(*) FROM sqlite_master WHERE type = 'table' and name = 'SchemaVersion';";
        using var reader = command.ExecuteReader();
        reader.Read();
        var count = reader.GetInt32(0);
        count.Should().Be(1);
    }

    [Fact]
    public void CheckSchemaVersionsExists_ShouldReturnFalse_WhenTableExists()
    {
        using SqliteConnection connection = CreateAndOpenSqliteConnection();
        CreateSchemaVersionsTable(connection);

        bool tableCreated = Program.CheckSchemaVersionsExists(connection);

        tableCreated.Should().BeFalse();
    }

    private static void CreateSchemaVersionsTable(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE SchemaVersion (ScriptName TEXT NOT NULL CONSTRAINT PK_SchemaVersion PRIMARY KEY, Applied TEXT NOT NULL DEFAULT CURRENT_DATETIME);";
        _ = command.ExecuteNonQuery();
        return;
    }

    private static SqliteConnection CreateAndOpenSqliteConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        return connection;
    }
}
