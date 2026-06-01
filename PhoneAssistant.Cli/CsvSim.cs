using FluentResults;

using Microsoft.VisualBasic.FileIO;

using System.Globalization;

namespace PhoneAssistant.Cli;

internal class CsvSim(string phoneNumber, string userName, ulong broadbandData, uint textMessages, uint voiceCalls)
{
    internal string PhoneNumber { get; init; } = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
    internal ulong BroadbandData { get; init; } = broadbandData;
    internal uint TextMessages { get; init; } = textMessages;
    internal string UserName { get; init; } = userName;
    internal uint VoiceCalls { get; init; } = voiceCalls;

    private static readonly string[] ExpectedHeader =
    [
        "Cost centre name",
        "Cost centre code",
        "Phone number",
        "User name",
        "Monthly recurring charges",
        "Other costs",
        "Call costs (airtime)",
        "Credits",
        "Total costs (excluding VAT)",
        "Cost of Voice",
        "Cost of SMS",
        "Cost of MMS",
        "Cost of Data",
        "Cost of GPRS",
        "Cost of Fax",
        "Cost of Email",
        "Number of voice calls",
        "Duration of voice calls",
        "Number of SMS",
        "Number of MMS (photo and video)",
        "Duration of data calls (CSD/HSCSD)",
        "Quantity of data (bytes)",
        "Quantity of GPRS data (bytes)",
        "Number of faxes",
        "Number of emails",
        "Duration of landline",
        "Duration of answerphone",
        "Number of text messages",
        "Duration of calls to EE mobiles (EE to EE)",
        "Duration of calls to other mobiles (other mobile network)",
        "Duration of calls to other",
        "Duration of calls to roaming",
        "Duration of calls to international",
        "Duration of calls to premium rate numbers",
        "Duration of calls to mobile voice VPN",
        "Invoice number",
        "Account/Group",
        "VAT exempt call costs (airtime)"
    ];

    internal static Result<string> ValidateHeader(string headerLine)
    {
        using var parser = new TextFieldParser(new StringReader(headerLine));
        parser.HasFieldsEnclosedInQuotes = true;
        parser.SetDelimiters(",");

        string[]? fields = parser.ReadFields();

        if (fields is null || fields.Length != ExpectedHeader.Length)
        {
            return Result.Fail<string>($"Header line has {fields?.Length ?? 0} columns, expected {ExpectedHeader.Length}");
        }

        for (int i = 0; i < ExpectedHeader.Length; i++)
        {
            if (fields[i].Trim('"') != ExpectedHeader[i])
            {
                return Result.Fail<string>($"Column {i} header mismatch. Expected '{ExpectedHeader[i]}', but got '{fields[i].Trim('"')}'");
            }
        }

        return Result.Ok(headerLine);
    }

    internal static Result<CsvSim> Parse(string csvLine)
    {
        using var parser = new TextFieldParser(new StringReader(csvLine));
        parser.HasFieldsEnclosedInQuotes = true;
        parser.SetDelimiters(",");

        string[]? fields = parser.ReadFields();

        if (fields is null || fields.Length < ExpectedHeader.Length)
            return Result.Fail<CsvSim>($"CSV line contains {fields?.Length ?? 0} columns, expected {ExpectedHeader.Length}.");

        string phoneNumber = fields[2];
        string userName = fields[3];

        string voice = fields[16].Trim('"');
        if (!uint.TryParse(voice, NumberStyles.AllowThousands, CultureInfo.GetCultureInfo("en-GB"), out var voiceCalls))
        {
            return Result.Fail<CsvSim>($"Unable to parse column '{ExpectedHeader[16]}'.");
        }
        string sms = fields[18].Trim('"');
        if (!uint.TryParse(sms, NumberStyles.AllowThousands, CultureInfo.GetCultureInfo("en-GB"), out var textMessages))
        {
            return Result.Fail<CsvSim>($"Unable to parse column '{ExpectedHeader[18]}'.");
        }

        string dataVolume = fields[21].Trim('"');
        if (!ulong.TryParse(dataVolume, NumberStyles.AllowThousands, CultureInfo.GetCultureInfo("en-GB"), out var broadbandData))
        {
            return Result.Fail<CsvSim>($"Unable to parse column '{ExpectedHeader[21]}'.");
        }

        return new CsvSim(phoneNumber, userName, broadbandData, textMessages, voiceCalls);
    }
}
