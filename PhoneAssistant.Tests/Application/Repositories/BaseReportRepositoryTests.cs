using PhoneAssistant.Model;

namespace PhoneAssistant.Tests.Application.Repositories;
public sealed class BaseReportRepositoryTests
{
    readonly DbTestHelper _helper = new();
    readonly BaseReportRepository _repository;

    public BaseReportRepositoryTests()
    {
        _repository = new(_helper.DbContext);
    }

    [Test]
    public async Task GetSimNumber_ShouldReturnNull_WhenSIMDoesNotExist()
    {
        string? actual = await _repository.GetSimNumberAsync("DoesNotExist");

        await Assert.That(actual).IsNull();
    }

    [Test]
    public async Task GetSimNumber_ShouldReturnSimNumber_WhenSimDoesExist()
    {
        _helper.DbContext.BaseReport.Add(new BaseReport() { PhoneNumber = "phonenumber", SimNumber = "sim number", ConnectedIMEI = "", ContractEndDate = "", Handset = "", LastUsedIMEI = "", TalkPlan = "", UserName = "" });
        await _helper.DbContext.SaveChangesAsync();

        string? actual = await _repository.GetSimNumberAsync("phonenumber");

        await Assert.That(actual).IsEqualTo("sim number");
    }
}