using CsvHelper;
using FluentResults;
using System.Globalization;

namespace PhoneAssistant.Cli.EECommand;

internal static class SIMDetailParser
{
    public static Result<Dictionary<string, SIMDetail>> LoadSimDetails(string filePath)
    {
        Dictionary<string, SIMDetail> simDetails = new(StringComparer.OrdinalIgnoreCase);

        using StreamReader reader = new(filePath);
        CsvReaderOptions options = new()
        {
            FormatProvider = CultureInfo.InvariantCulture
        };
        using CsvReader csv = CsvReader.Create(reader, options);

        var records = csv.GetRecords<SIMDetail>();
        try
        {
            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.PhoneNumber))
                    continue;

                simDetails[record.PhoneNumber] = record; 
            }
        }
        catch (CsvFormatException ex)
        {
            return Result.Fail(ex.Message);
        }

        return Result.Ok(simDetails);
    }
}

[CsvRecord]
internal sealed partial class SIMDetail
{
    [CsvColumn("Phone")]
    public required string PhoneNumber { get; init; }
    [CsvColumn("SIM number")]
    public required string SimNumber { get; init; }
    [CsvColumn("Label")]
    public required string UserName { get; init; }
    [CsvColumn("Active EID")]
    public required string EID { get; init; }
    [CsvColumn("SIM type")]
    public required string SimType { get; init; }
    [CsvColumn("Status")]
    public required string Status { get; init; }
    [CsvColumn("Plan")]
    public required string Plan { get; init; }
}
