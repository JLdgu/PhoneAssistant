using CsvHelper;
using CsvHelper.Configuration;
using FluentResults;
using System.Globalization;

namespace PhoneAssistant.Cli.EECommand;

internal static class SIMDetailParser
{
    public static Result<Dictionary<string, SIMDetail>> LoadSimDetails(string filePath)
    {
        Dictionary<string, SIMDetail> simDetails = new(StringComparer.OrdinalIgnoreCase);

        using StreamReader reader = new(filePath);
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<SIMDetailMap>();

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
        catch (HeaderValidationException ex)
        {

            return Result.Fail(ex.Message);
        }

        return Result.Ok(simDetails);
    }
}

internal sealed class SIMDetailMap : ClassMap<SIMDetail>
{
    public SIMDetailMap()
    {
        Map(m => m.PhoneNumber).Name("Phone");
        Map(m => m.SimNumber).Name("SIM number");
        Map(m => m.UserName).Name("Label");
        Map(m => m.EID).Name("Active EID");
        Map(m => m.SimType).Name("SIM type");
        Map(m => m.Status).Name("Status");
        Map(m => m.Plan).Name("Plan");
    }
}

internal sealed class SIMDetail
{
    public required string PhoneNumber { get; init; }
    public required string SimNumber { get; init; }
    public required string UserName { get; init; }
    public required string EID { get; init; }
    public required string SimType { get; init; }
    public required string Status { get; init; }
    public required string Plan { get; init; }
}
