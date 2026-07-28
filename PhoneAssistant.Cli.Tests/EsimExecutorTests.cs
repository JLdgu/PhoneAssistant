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
        IEnumerable<string> phoneNumber = ["PN"];
        IEnumerable<Sim> sims =
        [
            new() { BillingPeriod = "202602", Esim = true, PhoneNumber = "PN", SIMNumber = "Same", UserName = "John Doe", BroadbandData = 100, TextMessages = 50, VoiceCalls = 20 },
            new() { BillingPeriod = "202601", Esim = false, PhoneNumber = "PN", SIMNumber = "Same", UserName = "John Doe", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 },
            new() { BillingPeriod = "202512", Esim = false, PhoneNumber = "PN", SIMNumber = "Different ", UserName = "Bob Johnson", BroadbandData = 200, TextMessages = 100, VoiceCalls = 40 }
        ];
        AutoMocker mocker = new();
        Mock<ISimRepository> simsRepository = mocker.GetMock<ISimRepository>();
        simsRepository.Setup(x => x.GetSimsForPhoneNumber(It.IsAny<string>())).ReturnsAsync(sims);
        var sut = mocker.CreateInstance<EsimExecutor>();

        await EsimExecutor.CheckAndUpdateHistory(simsRepository.Object, phoneNumber);

        simsRepository.Verify(x => x.UpdateOrCreateAsync(It.Is<Sim>(s => s.BillingPeriod == "202601" && s.Esim == true)), Times.Once);
    }

    [Test]
    public async Task CheckAndUpdateHistory_should_ignore_sims_with_dirrent_SimNumber()
    {
        IEnumerable<string> phoneNumber = ["PN"];
        IEnumerable<Sim> sims =
        [
            new() { BillingPeriod = "202602", Esim = true, PhoneNumber = "PN", SIMNumber = "Same", UserName = "John Doe", BroadbandData = 100, TextMessages = 50, VoiceCalls = 20 },
            new() { BillingPeriod = "202601", Esim = false, PhoneNumber = "PN", SIMNumber = "Different", UserName = "Bob", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 },
            new() { BillingPeriod = "202512", Esim = false, PhoneNumber = "PN", SIMNumber = "Different ", UserName = "Bob", BroadbandData = 200, TextMessages = 100, VoiceCalls = 40 }
        ];
        AutoMocker mocker = new();
        Mock<ISimRepository> simsRepository = mocker.GetMock<ISimRepository>();
        simsRepository.Setup(x => x.GetSimsForPhoneNumber(It.IsAny<string>())).ReturnsAsync(sims);
        var sut = mocker.CreateInstance<EsimExecutor>();

        await EsimExecutor.CheckAndUpdateHistory(simsRepository.Object, phoneNumber);

        simsRepository.Verify(x => x.UpdateOrCreateAsync(It.Is<Sim>(s => s.BillingPeriod == "202601" && s.Esim == true)), Times.Never);
    }
}
