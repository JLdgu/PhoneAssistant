using FluentResults;

namespace PhoneAssistant.Cli.Tests;

public sealed class CsvSimTests
{
    [Test]
    public async Task Parse_ShouldSucceed_WithValidCsvLineAsync()
    {
        string csvLine = "\"Field0\",\"Field1\",\"07700900000\",\"John Doe\",\"£10.00\",\"Field5\",\"Field6\",\"Field7\",\"Field8\",\"Field9\",\"Field10\",\"Field11\",\"Field12\",\"Field13\",\"Field14\",\"Field15\",\"Field16\",\"Field17\",\"Field18\",\"Field19\",\"Field20\",\"5000\"";

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.PhoneNumber).IsEqualTo("07700900000");
        await Assert.That(result.Value.UserName).IsEqualTo("John Doe");
        await Assert.That(result.Value.RecurringCharge).IsEqualTo(10.00m);
        await Assert.That(result.Value.DataVolume).IsEqualTo(5000L);
    }

    [Test]
    public async Task Parse_ShouldThrow_WhenCsvLineHasFewerThanRequiredColumnsAsync()
    {
        string csvLine = "\"Field0\",\"Field1\",\"07700900000\",\"John Doe\"";

        await Assert.That(() => CsvSim.Parse(csvLine))
            .Throws<FormatException>()
            .WithMessageContaining("CSV line does not contain enough columns.");
    }

    [Test]
    public async Task Parse_ShouldFail_WhenRecurringChargeCannotBeParsedAsync()
    {
        string csvLine = "\"Field0\",\"Field1\",\"07700900000\",\"John Doe\",\"InvalidAmount\",\"Field5\",\"Field6\",\"Field7\",\"Field8\",\"Field9\",\"Field10\",\"Field11\",\"Field12\",\"Field13\",\"Field14\",\"Field15\",\"Field16\",\"Field17\",\"Field18\",\"Field19\",\"Field20\",\"5000\"";

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Unable to parse recurring charge value");
    }

    [Test]
    public async Task Parse_ShouldFail_WhenDataVolumeCannotBeParsedAsync()
    {
        string csvLine = "\"Field0\",\"Field1\",\"07700900000\",\"John Doe\",\"£10.00\",\"Field5\",\"Field6\",\"Field7\",\"Field8\",\"Field9\",\"Field10\",\"Field11\",\"Field12\",\"Field13\",\"Field14\",\"Field15\",\"Field16\",\"Field17\",\"Field18\",\"Field19\",\"Field20\",\"InvalidDataVolume\"";

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Unable to parse data volume value");
    }

    [Test]
    public async Task Parse_ShouldSucceed_WithThousandsSeparatorInDataVolumeAsync()
    {
        string csvLine = "\"Field0\",\"Field1\",\"07700900000\",\"John Doe\",\"£20.50\",\"Field5\",\"Field6\",\"Field7\",\"Field8\",\"Field9\",\"Field10\",\"Field11\",\"Field12\",\"Field13\",\"Field14\",\"Field15\",\"Field16\",\"Field17\",\"Field18\",\"Field19\",\"Field20\",\"1,000,000\"";

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.DataVolume).IsEqualTo(1000000L);
    }

    [Test]
    public async Task Parse_ShouldSucceed_WithZeroValuesAsync()
    {
        string csvLine = "\"Field0\",\"Field1\",\"07700900000\",\"User\",\"£0.00\",\"Field5\",\"Field6\",\"Field7\",\"Field8\",\"Field9\",\"Field10\",\"Field11\",\"Field12\",\"Field13\",\"Field14\",\"Field15\",\"Field16\",\"Field17\",\"Field18\",\"Field19\",\"Field20\",\"0\"";

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.RecurringCharge).IsEqualTo(0m);
        await Assert.That(result.Value.DataVolume).IsEqualTo(0L);
    }

    [Test]
    public async Task Constructor_ShouldThrowArgumentNullException_WhenPhoneNumberIsNullAsync()
    {
        await Assert.That(() => new CsvSim(null!, "John Doe", 10m, 5000L))
            .Throws<ArgumentNullException>()
            .WithMessageContaining("phoneNumber");
    }

    [Test]
    public async Task Properties_ShouldBeInitOnlyAsync()
    {
        var csvSim = new CsvSim("07700900000", "John Doe", 10m, 5000L);

        await Assert.That(csvSim.PhoneNumber).IsEqualTo("07700900000");
        await Assert.That(csvSim.UserName).IsEqualTo("John Doe");
        await Assert.That(csvSim.RecurringCharge).IsEqualTo(10m);
        await Assert.That(csvSim.DataVolume).IsEqualTo(5000L);
    }

    [Test]
    public async Task Parse_ShouldHandleDecimalWithCurrencySymbolAsync()
    {
        string csvLine = "\"Field0\",\"Field1\",\"07700900000\",\"John Doe\",\"£99.99\",\"Field5\",\"Field6\",\"Field7\",\"Field8\",\"Field9\",\"Field10\",\"Field11\",\"Field12\",\"Field13\",\"Field14\",\"Field15\",\"Field16\",\"Field17\",\"Field18\",\"Field19\",\"Field20\",\"2500\"";

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.RecurringCharge).IsEqualTo(99.99m);
    }

    [Test]
    public async Task Parse_ShouldExtractCorrectFieldIndicesAsync()
    {
        string csvLine = "\"Col0\",\"Col1\",\"07123456789\",\"Alice Smith\",\"£15.75\",\"Col5\",\"Col6\",\"Col7\",\"Col8\",\"Col9\",\"Col10\",\"Col11\",\"Col12\",\"Col13\",\"Col14\",\"Col15\",\"Col16\",\"Col17\",\"Col18\",\"Col19\",\"Col20\",\"7500\"";

        Result<CsvSim> result = CsvSim.Parse(csvLine);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.PhoneNumber).IsEqualTo("07123456789");
        await Assert.That(result.Value.UserName).IsEqualTo("Alice Smith");
        await Assert.That(result.Value.RecurringCharge).IsEqualTo(15.75m);
        await Assert.That(result.Value.DataVolume).IsEqualTo(7500L);
    }

    [Test]
    public async Task ValidateHeader_ShouldSucceed_WithCorrectHeaderAsync()
    {
        string headerLine = "\"Cost centre name\",\"Cost centre code\",\"Phone number\",\"User name\",\"Monthly recurring charges\",\"Other costs\",\"Call costs (airtime)\",\"Credits\",\"Total costs (excluding VAT)\",\"Cost of Voice\",\"Cost of SMS\",\"Cost of MMS\",\"Cost of Data\",\"Cost of GPRS\",\"Cost of Fax\",\"Cost of Email\",\"Number of voice calls\",\"Duration of voice calls\",\"Number of SMS\",\"Number of MMS (photo and video)\",\"Duration of data calls (CSD/HSCSD)\",\"Quantity of data (bytes)\",\"Quantity of GPRS data (bytes)\",\"Number of faxes\",\"Number of emails\",\"Duration of landline\",\"Duration of answerphone\",\"Number of text messages\",\"Duration of calls to EE mobiles (EE to EE)\",\"Duration of calls to other mobiles (other mobile network)\",\"Duration of calls to other\",\"Duration of calls to roaming\",\"Duration of calls to international\",\"Duration of calls to premium rate numbers\",\"Duration of calls to mobile voice VPN\",\"Invoice number\",\"Account/Group\",\"VAT exempt call costs (airtime)\"";

        Result<string> result = CsvSim.ValidateHeader(headerLine);

        await Assert.That(result.IsSuccess).IsTrue();
    }

    [Test]
    public async Task ValidateHeader_ShouldFail_WhenHeaderHasTooFewColumnsAsync()
    {
        string headerLine = "\"Cost centre name\",\"Cost centre code\",\"Phone number\"";

        Result<string> result = CsvSim.ValidateHeader(headerLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).Contains("expected 38");
    }

    [Test]
    public async Task ValidateHeader_ShouldFail_WhenHeaderHasTooManyColumnsAsync()
    {
        string headerLine = "\"Cost centre name\",\"Cost centre code\",\"Phone number\",\"User name\",\"Monthly recurring charges\",\"Other costs\",\"Call costs (airtime)\",\"Credits\",\"Total costs (excluding VAT)\",\"Cost of Voice\",\"Cost of SMS\",\"Cost of MMS\",\"Cost of Data\",\"Cost of GPRS\",\"Cost of Fax\",\"Cost of Email\",\"Number of voice calls\",\"Duration of voice calls\",\"Number of SMS\",\"Number of MMS (photo and video)\",\"Duration of data calls (CSD/HSCSD)\",\"Quantity of data (bytes)\",\"Quantity of GPRS data (bytes)\",\"Number of faxes\",\"Number of emails\",\"Duration of landline\",\"Duration of answerphone\",\"Number of text messages\",\"Duration of calls to EE mobiles (EE to EE)\",\"Duration of calls to other mobiles (other mobile network)\",\"Duration of calls to other\",\"Duration of calls to roaming\",\"Duration of calls to international\",\"Duration of calls to premium rate numbers\",\"Duration of calls to mobile voice VPN\",\"Invoice number\",\"Account/Group\",\"VAT exempt call costs (airtime)\",\"Extra column\"";

        Result<string> result = CsvSim.ValidateHeader(headerLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).Contains("expected 38");
    }

    [Test]
    public async Task ValidateHeader_ShouldFail_WhenColumnHeaderMismatchesAsync()
    {
        string headerLine = "\"Cost centre name\",\"Cost centre code\",\"Phone number\",\"User name\",\"WRONG_HEADER\",\"Other costs\",\"Call costs (airtime)\",\"Credits\",\"Total costs (excluding VAT)\",\"Cost of Voice\",\"Cost of SMS\",\"Cost of MMS\",\"Cost of Data\",\"Cost of GPRS\",\"Cost of Fax\",\"Cost of Email\",\"Number of voice calls\",\"Duration of voice calls\",\"Number of SMS\",\"Number of MMS (photo and video)\",\"Duration of data calls (CSD/HSCSD)\",\"Quantity of data (bytes)\",\"Quantity of GPRS data (bytes)\",\"Number of faxes\",\"Number of emails\",\"Duration of landline\",\"Duration of answerphone\",\"Number of text messages\",\"Duration of calls to EE mobiles (EE to EE)\",\"Duration of calls to other mobiles (other mobile network)\",\"Duration of calls to other\",\"Duration of calls to roaming\",\"Duration of calls to international\",\"Duration of calls to premium rate numbers\",\"Duration of calls to mobile voice VPN\",\"Invoice number\",\"Account/Group\",\"VAT exempt call costs (airtime)\"";

        Result<string> result = CsvSim.ValidateHeader(headerLine);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).Contains("Column 4 header mismatch");
        await Assert.That(result.Errors[0].Message).Contains("Monthly recurring charges");
        await Assert.That(result.Errors[0].Message).Contains("WRONG_HEADER");
    }
}
