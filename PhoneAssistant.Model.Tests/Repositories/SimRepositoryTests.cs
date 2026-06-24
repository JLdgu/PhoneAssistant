namespace PhoneAssistant.Model.Tests; 

public class SimRepositoryTests
{
    readonly DbTestHelper _helper = new();
    readonly SimRepository _repository;

    public SimRepositoryTests()
    {
        _repository = new(_helper.DbContext);
    }
        
    [Test]
    public async Task GetLatestBillingPeriod_should_return_Latest_when_Sims_exist()
    {
        Sim sim1 = new() { BillingPeriod = "202601", SIMNumber = "8944122605563572205", PhoneNumber = "07814209742", UserName = "John Doe", BroadbandData = 100 , TextMessages = 50, VoiceCalls = 20 };
        Sim sim2 = new() { BillingPeriod = "202602", SIMNumber = "8944122605563572206", PhoneNumber = "07814209743", UserName = "Jane Smith", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 };
        Sim sim3 = new() { BillingPeriod = "202501", SIMNumber = "8944122605563572207", PhoneNumber = "07814209744", UserName = "Bob Johnson", BroadbandData = 200, TextMessages = 100, VoiceCalls = 40 };

        _helper.DbContext.Sims.Add(sim1);
        _helper.DbContext.Sims.Add(sim2);
        await _helper.DbContext.SaveChangesAsync();

        var actual = await _repository.GetLatestBillingPeriod();

        await Assert.That(actual).IsEqualTo("202602");
    }

    [Test]
    public async Task GetLatestBillingPeriod_should_return_Unknown_when_no_Sims_exist()
    {
        var actual = await _repository.GetLatestBillingPeriod();

        await Assert.That(actual).IsEqualTo("Unknown");
    }

    [Test]
    public async Task GetSimNumberAsync_should_return_Sims_for_given_phoneNumber()
    {
        Sim sim1 = new() { BillingPeriod = "202601", SIMNumber = "8944122605563572205", PhoneNumber = "07814209742", UserName = "John Doe", BroadbandData = 100, TextMessages = 50, VoiceCalls = 20 };
        Sim sim2 = new() { BillingPeriod = "202602", SIMNumber = "8944122605563572206", PhoneNumber = "07814209742", UserName = "Jane Smith", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 };
        Sim sim3 = new() { BillingPeriod = "202501", SIMNumber = "8944122605563572207", PhoneNumber = "07814209744", UserName = "Bob Johnson", BroadbandData = 200, TextMessages = 100, VoiceCalls = 40 };
        _helper.DbContext.Sims.Add(sim1);
        _helper.DbContext.Sims.Add(sim2);
        _helper.DbContext.Sims.Add(sim3);

        await _helper.DbContext.SaveChangesAsync();

        string? simNumber = await _repository.GetSimNumberAsync("07814209742");
        await Assert.That(simNumber).IsEqualTo("8944122605563572206");
    }
}
