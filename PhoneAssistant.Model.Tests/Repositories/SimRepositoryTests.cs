using Microsoft.EntityFrameworkCore;

namespace PhoneAssistant.Model.Tests; 

internal class SimRepositoryTests
{
    readonly DbTestHelper _helper = new();
    readonly SimRepository _repository;

    internal SimRepositoryTests()
    {
        _repository = new(_helper.DbContext);
    }
        
    [Test]
    internal async Task GetLatestBillingPeriod_should_return_Latest_when_Sims_exist()
    {
        Sim sim1 = new() { BillingPeriod = "202601", SIMNumber = "8944122605563572205", PhoneNumber = "07814209742", UserName = "John Doe", BroadbandData = 100 , TextMessages = 50, VoiceCalls = 20 };
        Sim sim2 = new() { BillingPeriod = "202602", SIMNumber = "8944122605563572206", PhoneNumber = "07814209743", UserName = "Jane Smith", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 };
        Sim sim3 = new() { BillingPeriod = "202501", SIMNumber = "8944122605563572207", PhoneNumber = "07814209744", UserName = "Bob Johnson", BroadbandData = 200, TextMessages = 100, VoiceCalls = 40 };

        _helper.DbContext.Sims.Add(sim1);
        _helper.DbContext.Sims.Add(sim2);
        _helper.DbContext.Sims.Add(sim3);
        await _helper.DbContext.SaveChangesAsync();

        var actual = await _repository.GetLatestBillingPeriod();

        await Assert.That(actual).IsEqualTo("202602");
    }

    [Test]
    internal async Task GetLatestBillingPeriod_should_return_Unknown_when_no_Sims_exist()
    {
        var actual = await _repository.GetLatestBillingPeriod();

        await Assert.That(actual).IsEqualTo("Unknown");
    }

    [Test]
    internal async Task GetEsim_should_return_distinct_list_of_Esims()
    {
        Sim sim1 = new() { BillingPeriod = "202601", Esim = false, PhoneNumber = "07814209742", SIMNumber = "8944122605563572205", UserName = "John Doe", BroadbandData = 100, TextMessages = 50, VoiceCalls = 20 };
        Sim sim2 = new() { BillingPeriod = "202602", Esim = true, PhoneNumber = "07814209742", SIMNumber = "8944122605563572206", UserName = "John Doe", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 };
        Sim sim3 = new() { BillingPeriod = "202501", Esim = false, PhoneNumber = "07814209744", SIMNumber = "8944122605563572207", UserName = "Bob Johnson", BroadbandData = 200, TextMessages = 100, VoiceCalls = 40 };

        _helper.DbContext.Sims.Add(sim1);
        _helper.DbContext.Sims.Add(sim2);
        _helper.DbContext.Sims.Add(sim3);
        await _helper.DbContext.SaveChangesAsync();

        IEnumerable<string> phoneNumbers = await _repository.GetEsims();
        await Assert.That(phoneNumbers.Count()).IsEqualTo(1);
    }

    [Test]
    internal async Task GetSimNumberAsync_should_return_latest_SimNumber_for_given_phoneNumber()
    {
        Sim sim1 = new() { BillingPeriod = "202601", SIMNumber = "8944122605563572205", PhoneNumber = "07814209742", UserName = "John Doe", BroadbandData = 100, TextMessages = 50, VoiceCalls = 20 };
        Sim sim2 = new() { BillingPeriod = "202602", SIMNumber = "8944122605563572206", PhoneNumber = "07814209742", UserName = "Jane Smith", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 };
        Sim sim3 = new() { BillingPeriod = "202501", SIMNumber = "8944122605563572207", PhoneNumber = "07814209744", UserName = "Bob Johnson", BroadbandData = 200, TextMessages = 100, VoiceCalls = 40 };
        _helper.DbContext.Sims.Add(sim1);
        _helper.DbContext.Sims.Add(sim2);
        _helper.DbContext.Sims.Add(sim3);
        await _helper.DbContext.SaveChangesAsync();

        string? simNumber = await _repository.GetSimNumber("07814209742");
        await Assert.That(simNumber).IsEqualTo("8944122605563572206");
    }    

    [Test]
    internal async Task GetSimsForPhoneNumber_should_return_Sims_for_given_phoneNumber()
    {
        Sim sim1 = new() { BillingPeriod = "202601", SIMNumber = "8944122605563572205", PhoneNumber = "07814209742", UserName = "John Doe", BroadbandData = 100, TextMessages = 50, VoiceCalls = 20 };
        Sim sim2 = new() { BillingPeriod = "202602", SIMNumber = "8944122605563572206", PhoneNumber = "07814209742", UserName = "Jane Smith", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 };
        Sim sim3 = new() { BillingPeriod = "202501", SIMNumber = "8944122605563572207", PhoneNumber = "07814209744", UserName = "Bob Johnson", BroadbandData = 200, TextMessages = 100, VoiceCalls = 40 };
        _helper.DbContext.Sims.Add(sim1);
        _helper.DbContext.Sims.Add(sim2);
        _helper.DbContext.Sims.Add(sim3);
        await _helper.DbContext.SaveChangesAsync();

        IEnumerable<Sim> sims = await _repository.GetSimsForPhoneNumber("07814209742");
        
        Sim? firstSim = sims.FirstOrDefault();
        await Assert.That(sims).Count().IsEqualTo(2);
        await Assert.That(firstSim).IsNotNull();
        await Assert.That(firstSim.BillingPeriod).IsEqualTo("202602");
    }

    [Test]
    internal async Task GetSimsForSimNumber_should_return_latest_Sims_for_given_SimNumber()
    {
        Sim sim1 = new() { BillingPeriod = "202601", SIMNumber = "8944122605563572205", PhoneNumber = "07814209742", UserName = "John Doe", BroadbandData = 100, TextMessages = 50, VoiceCalls = 20 };
        Sim sim2 = new() { BillingPeriod = "202602", SIMNumber = "8944122605563572206", PhoneNumber = "07814209742", UserName = "Jane Smith", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 };
        Sim sim3 = new() { BillingPeriod = "202603", SIMNumber = "8944122605563572206", PhoneNumber = "07814209742", UserName = "Jane Smith", BroadbandData = 50, TextMessages = 5, VoiceCalls = 3 };
        Sim sim4 = new() { BillingPeriod = "202501", SIMNumber = "8944122605563572207", PhoneNumber = "07814209744", UserName = "Bob Johnson", BroadbandData = 200, TextMessages = 100, VoiceCalls = 40 };
        _helper.DbContext.Sims.Add(sim1);
        _helper.DbContext.Sims.Add(sim2);
        _helper.DbContext.Sims.Add(sim3);
        _helper.DbContext.Sims.Add(sim4);
        await _helper.DbContext.SaveChangesAsync();

        IEnumerable<Sim> simsFullNumber = await _repository.GetSimsForSimNumber("8944122605563572206");

        Sim? firstSim = simsFullNumber.FirstOrDefault();
        await Assert.That(simsFullNumber).Count().IsEqualTo(2);
        await Assert.That(firstSim).IsNotNull();
        await Assert.That(firstSim.BillingPeriod).IsEqualTo("202603");
    }

    [Test]
    internal async Task GetSimsForUserName_should_return_latest_Sims_for_given_SimNumber()
    {
        Sim sim1 = new() { BillingPeriod = "202601", SIMNumber = "8944122605563572205", PhoneNumber = "07814209742", UserName = "John Doe", BroadbandData = 100, TextMessages = 50, VoiceCalls = 20 };
        Sim sim2 = new() { BillingPeriod = "202602", SIMNumber = "8944122605563572206", PhoneNumber = "07814209742", UserName = "Jane Smith", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 };
        Sim sim3 = new() { BillingPeriod = "202603", SIMNumber = "8944122605563572206", PhoneNumber = "07814209744", UserName = "Jane Smith", BroadbandData = 50, TextMessages = 5, VoiceCalls = 3 };
        Sim sim4 = new() { BillingPeriod = "202501", SIMNumber = "8944122605563572207", PhoneNumber = "07814209744", UserName = "Bob Johnson", BroadbandData = 200, TextMessages = 100, VoiceCalls = 40 };
        _helper.DbContext.Sims.Add(sim1);
        _helper.DbContext.Sims.Add(sim2);
        _helper.DbContext.Sims.Add(sim3);
        _helper.DbContext.Sims.Add(sim4);
        await _helper.DbContext.SaveChangesAsync();

        IEnumerable<Sim> simsFullNumber = await _repository.GetSimsForUserName("Jane Smith");

        Sim? firstSim = simsFullNumber.FirstOrDefault();
        await Assert.That(simsFullNumber).Count().IsEqualTo(2);
        await Assert.That(firstSim).IsNotNull();
        await Assert.That(firstSim.BillingPeriod).IsEqualTo("202603");
    }

    [Test]
    internal async Task UpdateOrCreateAsync_should_create_new_Sim()
    {
        await _repository.UpdateOrCreateAsync(new Sim { BillingPeriod = "BP", PhoneNumber = "PN", SIMNumber = "sim_number", UserName = "user_name", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 });

        Sim? updatedSim = _helper.DbContext.Sims.FirstOrDefault();
        await Assert.That(_helper.DbContext.Sims).Count().IsEqualTo(1);
        await Assert.That(updatedSim).IsNotNull();
        await Assert.That(updatedSim.SIMNumber).IsEqualTo("sim_number");
        await Assert.That(updatedSim.UserName).IsEqualTo("user_name");
    }

    [Test]
    internal async Task UpdateOrCreateAsync_should_update_existing_Sim()
    {
        Sim sim = new() { BillingPeriod = "202601", PhoneNumber = "07814209742", SIMNumber = "original_sim_number", UserName = "original_user_name", BroadbandData = 100, TextMessages = 50, VoiceCalls = 20 };
        _helper.DbContext.Sims.Add(sim);
        await _helper.DbContext.SaveChangesAsync();
        _helper.DbContext.Entry(sim).State = EntityState.Detached;

        await _repository.UpdateOrCreateAsync(new Sim {BillingPeriod = sim.BillingPeriod, SIMNumber = "updated_sim_number", PhoneNumber = sim.PhoneNumber, UserName = "updated_user_name", BroadbandData = 150, TextMessages = 75, VoiceCalls = 30 });

        Sim? updatedSim = _helper.DbContext.Sims.FirstOrDefault();
        await Assert.That(_helper.DbContext.Sims).Count().IsEqualTo(1);
        await Assert.That(updatedSim).IsNotNull();
        await Assert.That(updatedSim.SIMNumber).IsEqualTo("updated_sim_number");
        await Assert.That(updatedSim.UserName).IsEqualTo("updated_user_name");
    }
}
