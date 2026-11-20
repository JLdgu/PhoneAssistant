using CsvHelper;
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
        CsvReaderOptions options = new()
        {
            FormatProvider = CultureInfo.InvariantCulture
        };
        using var csv = CsvReader.Create(reader, options);

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
        catch (CsvFormatException ex)
        {
            return Result.Fail(ex.Message);
        }

        return Result.Ok(phoneSummaries);
    }
}


[CsvRecord]
internal sealed partial class PhoneSummary
{
    [CsvColumn("Phone number")]
    public required string PhoneNumber { get; init; }
    [CsvColumn("Quantity of data (bytes)")]
    [CsvConverter(typeof(UlongThousandsConverter))]
    public required ulong BroadbandData { get; init; }
    [CsvColumn("Number of SMS")]
    [CsvConverter(typeof(UintThousandsConverter))]
    public required uint TextMessageCount { get; init; }
    [CsvColumn("User name")]
    public required string UserName { get; init; }
    [CsvColumn("Number of voice calls")]
    [CsvConverter(typeof(UintThousandsConverter))]
    public required uint VoiceCallCount { get; init; }
}

internal sealed class UlongThousandsConverter : ICsvConverter<ulong>
{
    public ulong Read(CsvReader reader, int index)
        => ulong.Parse(reader.GetString(index), NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

    public void Write(CsvWriter writer, ulong value)
        => writer.WriteField(value);
}

internal sealed class UintThousandsConverter : ICsvConverter<uint>
{
    public uint Read(CsvReader reader, int index)
        => uint.Parse(reader.GetString(index), NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

    public void Write(CsvWriter writer, uint value)
        => writer.WriteField(value);
}
