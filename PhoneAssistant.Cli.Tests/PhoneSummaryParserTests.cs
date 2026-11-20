using System.IO.Compression;
using System.Text;

using PhoneAssistant.Cli.EECommand;

namespace PhoneAssistant.Cli.Tests;

public sealed class PhoneSummaryParserTests
{
    [Test]
    public async Task LoadPhoneSummaries_ShouldParseThousandsSeparatorsAsync()
    {
        using MemoryStream archiveStream = new();
        using (ZipArchive archive = new(archiveStream, ZipArchiveMode.Create, true))
        {
            ZipArchiveEntry entry = archive.CreateEntry("PhoneSummary.csv");
            using Stream entryStream = entry.Open();
            using StreamWriter writer = new(entryStream, Encoding.Latin1);
            await writer.WriteLineAsync(
                "Phone number,Quantity of data (bytes),Number of SMS,User name,Number of voice calls");
            await writer.WriteLineAsync(
                "07123456789,\"1,234,567,890\",\"12,345\",Alice,\"987,654\"");
        }

        archiveStream.Position = 0;
        using ZipArchive readArchive = new(archiveStream, ZipArchiveMode.Read);
        ZipArchiveEntry csvEntry = readArchive.GetEntry("PhoneSummary.csv")!;

        var result = PhoneSummaryParser.LoadPhoneSummaries(csvEntry);

        await Assert.That(result.IsSuccess).IsTrue();
        PhoneSummary summary = result.Value["07123456789"];
        await Assert.That(summary.BroadbandData).IsEqualTo(1_234_567_890UL);
        await Assert.That(summary.TextMessageCount).IsEqualTo(12_345U);
        await Assert.That(summary.VoiceCallCount).IsEqualTo(987_654U);
    }
}
