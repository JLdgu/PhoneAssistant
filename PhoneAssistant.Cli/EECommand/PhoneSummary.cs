using CsvHelper;
using CsvHelper.Configuration;
using FluentResults;
using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace PhoneAssistant.Cli.EECommand;

internal static class PhoneSummaryParser
{
    public static Result<Dictionary<string, PhoneSummary>> LoadPhoneSummaries(ZipArchiveEntry zipEntry)
    {
        Dictionary<string, PhoneSummary> phoneSummaries = new(StringComparer.OrdinalIgnoreCase);

        using var reader = new StreamReader(zipEntry.Open(), Encoding.Latin1);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<PhoneSummaryMap>();

        var records = csv.GetRecords<PhoneSummary>();

        try
        {
            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.PhoneNumber))
                    continue;

                phoneSummaries[record.PhoneNumber] = record;
            }
        }
        catch (HeaderValidationException ex)
        {

            return Result.Fail(ex.Message);
        }

        return Result.Ok(phoneSummaries);
    }
}


internal sealed class PhoneSummaryMap : ClassMap<PhoneSummary>
{
    public PhoneSummaryMap()
    {
        Map(m => m.TextMessageCount).Name("Number of SMS").TypeConverterOption.NumberStyles(NumberStyles.AllowThousands);
        Map(m => m.VoiceCallCount).Name("Number of voice calls").TypeConverterOption.NumberStyles(NumberStyles.AllowThousands);
        Map(m => m.PhoneNumber).Name("Phone number");
        Map(m => m.BroadbandData).Name("Quantity of data (bytes)").TypeConverterOption.NumberStyles(NumberStyles.AllowThousands);
        Map(m => m.UserName).Name("User name");
    }
}

internal sealed class PhoneSummary
{
    public required string PhoneNumber { get; init; }
    public required ulong BroadbandData { get; init; }
    public required uint TextMessageCount { get; init; }
    public required string UserName { get; init; }
    public required uint VoiceCallCount { get; init; }
}
