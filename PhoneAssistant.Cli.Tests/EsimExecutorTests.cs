using DocumentFormat.OpenXml.Drawing.Charts;

using Moq;
using Moq.AutoMock;

using PhoneAssistant.Cli.EsimCommand;
using PhoneAssistant.Model;

namespace PhoneAssistant.Cli.Tests;

internal class EsimExecutorTests
{

    [Test]
    [Arguments("202511", "202601")]
    [Arguments("202512", "202601")]
    [Arguments("202601", "202601")]
    [Arguments("202602", "202603")]
    [Arguments("202603", "202603")]
    [Arguments("202604", "202605")]
    [Arguments("202605", "202605")]
    [Arguments("202606", null)]
    public async Task BP_MS(string actual, string? expected)
    {
        var ms = new List<string> {  "202605", "202603", "202601" };

        var result1 = ms
            .OrderBy(p => p)
            .Where(p => string.Compare(actual, p, StringComparison.Ordinal) <= 0)
            .FirstOrDefault();
                
        await Assert.That(result1).IsEqualTo(expected);

    }

    [Test]
    public async Task CheckAndUpdateHistory_should_update_sims_with_same_SimNumber()
    {
        IEnumerable<Tuple<string, string>> phoneNumber = [new Tuple<string, string>("PN", "Same")];
        IEnumerable<Sim> sims =
        [
            new() { BillingPeriod = "202601", Esim = false, PhoneNumber = "PN", SIMNumber = "SN", UserName = "John Doe", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 },
        ];
        AutoMocker mocker = new();
        Mock<ISimRepository> simsRepository = mocker.GetMock<ISimRepository>();
        simsRepository.Setup(x => x.GetPhysicalSimsForPhoneNumberAndSimNumber(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(sims);
        var sut = mocker.CreateInstance<EsimExecutor>();

        await EsimExecutor.CheckAndUpdateHistory(simsRepository.Object, phoneNumber);

        simsRepository.Verify(x => x.UpdateOrCreateAsync(It.Is<Sim>(s => s.BillingPeriod == "202601" && s.Esim == true)), Times.Once);
    }

    [Test]
    public async Task CheckAndUpdateHistory_should_ignore_sims_with_dirrent_SimNumber()
    {
        IEnumerable<Tuple<string, string>> phoneNumber = [new Tuple<string, string>("PN", "Same")];
        IEnumerable<Sim> sims = [];
        AutoMocker mocker = new();
        Mock<ISimRepository> simsRepository = mocker.GetMock<ISimRepository>();
        simsRepository.Setup(x => x.GetPhysicalSimsForPhoneNumberAndSimNumber(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(sims);
        var sut = mocker.CreateInstance<EsimExecutor>();

        await EsimExecutor.CheckAndUpdateHistory(simsRepository.Object, phoneNumber);

        simsRepository.Verify(x => x.UpdateOrCreateAsync(It.Is<Sim>(s => s.BillingPeriod == "202601" && s.Esim == true)), Times.Never);
    }
}
