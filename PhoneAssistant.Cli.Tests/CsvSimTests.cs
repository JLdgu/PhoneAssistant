using FluentResults;

namespace PhoneAssistant.Cli.Tests;

public sealed class CsvSimTests
{
    [Test]
    public async Task Parse_ShouldThrow_WhenCsvLineHasFewerThanRequiredColumns()
    {
        string csvLine = """
            "F0","F1","07700900000","John Doe"
            """;
            

        Result<CsvSim> result = CsvSim.Parse(csvLine);
        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("CSV line contains 4 columns, expected 38.");
    }

    [Test]
    public async Task Parse_ShouldFail_WhenBroadbandDataCannotBeParsed()
    {
        string csvLine = """
            "F0","F1","07700900000","John Doe","£10.00","F5","F6","F7","F8","F9","F10","F11","F12","F13","F14","F15","20","F17","1,234","F19","F20","nan","F22","F23","F24","F25","F26","F27","F28","F29","F30","F31","F32","F33","F34","F35","F36","F37"
            """;

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Unable to parse column 'Quantity of data (bytes)'.");
    }

    [Test]
    public async Task Parse_ShouldFail_WhenTextMessagesCannotBeParsed()
    {
        string csvLine = """
            "F0","F1","07700900000","John Doe","£10.00","F5","F6","F7","F8","F9","F10","F11","F12","F13","F14","F15","20","F17","nan","F19","F20","5,000","F22","F23","F24","F25","F26","F27","F28","F29","F30","F31","F32","F33","F34","F35","F36","F37"
            """;

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Unable to parse column 'Number of SMS'.");
    }

    [Test]
    public async Task Parse_ShouldFail_WhenVoiceCallsCannotBeParsed()
    {
        string csvLine = """
            "F0","F1","07700900000","John Doe","£10.00","F5","F6","F7","F8","F9","F10","F11","F12","F13","F14","F15","nan","F17","1,234","F19","F20","5,000","F22","F23","F24","F25","F26","F27","F28","F29","F30","F31","F32","F33","F34","F35","F36","F37"
            """;

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Unable to parse column 'Number of voice calls'.");
    }

    [Test]
    public async Task Parse_ShouldSucceed_WithValidCsvLine()
    {
        string csvLine = """
            "F0","F1","07700900000","John Doe","£10.00","F5","F6","F7","F8","F9","F10","F11","F12","F13","F14","F15","20","F17","1,234","F19","F20","5000","F22","F23","F24","F25","F26","F27","F28","F29","F30","F31","F32","F33","F34","F35","F36","F37"
            """;


        Result<CsvSim> result = CsvSim.Parse(csvLine);
        Console.WriteLine(result.Value);
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.PhoneNumber).IsEqualTo("07700900000");
        await Assert.That(result.Value.UserName).IsEqualTo("John Doe");
        await Assert.That(result.Value.BroadbandData).IsEqualTo(5000UL);
        await Assert.That(result.Value.TextMessages).IsEqualTo(1234U);
        await Assert.That(result.Value.VoiceCalls).IsEqualTo(20U);
    }


    [Test]
    public async Task Parse_ShouldSucceed_WithThousandsSeparatorInDataVolume()
    {
        string csvLine = """
            "F0","F1","07700900000","John Doe","£10.00","F5","F6","F7","F8","F9","F10","F11","F12","F13","F14","F15","20","F17","1,234","F19","F20","1,234,567","F22","F23","F24","F25","F26","F27","F28","F29","F30","F31","F32","F33","F34","F35","F36","F37"
            """;

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.BroadbandData).IsEqualTo(1234567UL);
    }

    [Test]
    public async Task Parse_ShouldSucceed_WithZeroValues()
    {
        string csvLine = """
            "F0","F1","07700900000","John Doe","£10.00","F5","F6","F7","F8","F9","F10","F11","F12","F13","F14","F15","0","F17","0","F19","F20","0","F22","F23","F24","F25","F26","F27","F28","F29","F30","F31","F32","F33","F34","F35","F36","F37"
            """;

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.BroadbandData).IsEqualTo(0UL);
        await Assert.That(result.Value.TextMessages).IsEqualTo(0U);
        await Assert.That(result.Value.VoiceCalls).IsEqualTo(0U);
    }

    [Test]
    public async Task Constructor_ShouldThrowArgumentNullException_WhenPhoneNumberIsNull()
    {
        await Assert.That(() => new CsvSim(null!, "John Doe", 5000L, 100, 500))
            .Throws<ArgumentNullException>()
            .WithMessageContaining("phoneNumber");
    }   

    [Test]
    public async Task ValidateHeader_ShouldSucceed_WithCorrectHeader()
    {
        string headerLine = "\"Cost centre name\",\"Cost centre code\",\"Phone number\",\"User name\",\"Monthly recurring charges\",\"Other costs\",\"Call costs (airtime)\",\"Credits\",\"Total costs (excluding VAT)\",\"Cost of Voice\",\"Cost of SMS\",\"Cost of MMS\",\"Cost of Data\",\"Cost of GPRS\",\"Cost of Fax\",\"Cost of Email\",\"Number of voice calls\",\"Duration of voice calls\",\"Number of SMS\",\"Number of MMS (photo and video)\",\"Duration of data calls (CSD/HSCSD)\",\"Quantity of data (bytes)\",\"Quantity of GPRS data (bytes)\",\"Number of faxes\",\"Number of emails\",\"Duration of landline\",\"Duration of answerphone\",\"Number of text messages\",\"Duration of calls to EE mobiles (EE to EE)\",\"Duration of calls to other mobiles (other mobile network)\",\"Duration of calls to other\",\"Duration of calls to roaming\",\"Duration of calls to international\",\"Duration of calls to premium rate numbers\",\"Duration of calls to mobile voice VPN\",\"Invoice number\",\"Account/Group\",\"VAT exempt call costs (airtime)\"";

        Result<string> result = CsvSim.ValidateHeader(headerLine);

        await Assert.That(result.IsSuccess).IsTrue();
    }

    [Test]
    public async Task ValidateHeader_ShouldFail_WhenHeaderHasTooFewColumns()
    {
        string headerLine = "\"Cost centre name\",\"Cost centre code\",\"Phone number\"";

        Result<string> result = CsvSim.ValidateHeader(headerLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).Contains("expected 38");
    }

    [Test]
    public async Task ValidateHeader_ShouldFail_WhenHeaderHasTooManyColumns()
    {
        string headerLine = "\"Cost centre name\",\"Cost centre code\",\"Phone number\",\"User name\",\"Monthly recurring charges\",\"Other costs\",\"Call costs (airtime)\",\"Credits\",\"Total costs (excluding VAT)\",\"Cost of Voice\",\"Cost of SMS\",\"Cost of MMS\",\"Cost of Data\",\"Cost of GPRS\",\"Cost of Fax\",\"Cost of Email\",\"Number of voice calls\",\"Duration of voice calls\",\"Number of SMS\",\"Number of MMS (photo and video)\",\"Duration of data calls (CSD/HSCSD)\",\"Quantity of data (bytes)\",\"Quantity of GPRS data (bytes)\",\"Number of faxes\",\"Number of emails\",\"Duration of landline\",\"Duration of answerphone\",\"Number of text messages\",\"Duration of calls to EE mobiles (EE to EE)\",\"Duration of calls to other mobiles (other mobile network)\",\"Duration of calls to other\",\"Duration of calls to roaming\",\"Duration of calls to international\",\"Duration of calls to premium rate numbers\",\"Duration of calls to mobile voice VPN\",\"Invoice number\",\"Account/Group\",\"VAT exempt call costs (airtime)\",\"Extra column\"";

        Result<string> result = CsvSim.ValidateHeader(headerLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).Contains("expected 38");
    }

    [Test]
    public async Task ValidateHeader_ShouldFail_WhenColumnHeaderMismatches()
    {
        string headerLine = "\"Cost centre name\",\"Cost centre code\",\"Phone number\",\"User name\",\"WRONG_HEADER\",\"Other costs\",\"Call costs (airtime)\",\"Credits\",\"Total costs (excluding VAT)\",\"Cost of Voice\",\"Cost of SMS\",\"Cost of MMS\",\"Cost of Data\",\"Cost of GPRS\",\"Cost of Fax\",\"Cost of Email\",\"Number of voice calls\",\"Duration of voice calls\",\"Number of SMS\",\"Number of MMS (photo and video)\",\"Duration of data calls (CSD/HSCSD)\",\"Quantity of data (bytes)\",\"Quantity of GPRS data (bytes)\",\"Number of faxes\",\"Number of emails\",\"Duration of landline\",\"Duration of answerphone\",\"Number of text messages\",\"Duration of calls to EE mobiles (EE to EE)\",\"Duration of calls to other mobiles (other mobile network)\",\"Duration of calls to other\",\"Duration of calls to roaming\",\"Duration of calls to international\",\"Duration of calls to premium rate numbers\",\"Duration of calls to mobile voice VPN\",\"Invoice number\",\"Account/Group\",\"VAT exempt call costs (airtime)\"";

        Result<string> result = CsvSim.ValidateHeader(headerLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).Contains("Column 4 header mismatch");
        await Assert.That(result.Errors[0].Message).Contains("Monthly recurring charges");
        await Assert.That(result.Errors[0].Message).Contains("WRONG_HEADER");
    }
}
