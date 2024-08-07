using PhoneAssistant.WPF.Application.Entities;

using Xunit;
using Xunit.Abstractions;

namespace PhoneAssistant.Tests.Application;
public class DbContextTriggerTests(ITestOutputHelper output) : DbTestHelper
{
    readonly DbTestHelper _helper = new();

    [Fact]
    public async Task AddPhone_ShouldSetLastUpdate()
    {
        Phone phone = new() { Condition="R", Imei="imei", Model="model", OEM= OEMs.Nokia, Status="Production"};
        Assert.Empty(phone.LastUpdate);

        await _helper.DbContext.Phones.AddAsync(phone);
        await _helper.DbContext.SaveChangesAsync();

        Assert.NotEmpty(phone.LastUpdate);
    }

    [Fact]
    public async Task UpdatePhone_ShouldChangeLastUpdate()
    {
        Phone phone = new() { Condition = "R", Imei = "imei", Model = "model", OEM = OEMs.Nokia, Status = "Production" };
        await _helper.DbContext.Phones.AddAsync(phone);
        await _helper.DbContext.SaveChangesAsync();
        string addDateTime = phone.LastUpdate;
        phone.AssetTag = "AssetTag";

        await Task.Delay(1000); // make sure timestamps differ
        _helper.DbContext.Phones.Update(phone);
        await _helper.DbContext.SaveChangesAsync();

        Assert.NotEqual(addDateTime, phone.LastUpdate);
    }

    [Fact]
    public async Task AddSim_ShouldSetLastUpdate()
    {

        Sim sim = new() { PhoneNumber = "123456789", SimNumber = "1123456789" };
        Assert.Empty(sim.LastUpdate);

        await _helper.DbContext.Sims.AddAsync(sim);
        await _helper.DbContext.SaveChangesAsync();

        Assert.NotEmpty(sim.LastUpdate);
    }

    [Fact]
    public async Task UpdateSim_ShouldChangeLastUpdate()
    {

        Sim sim = new() { PhoneNumber = "123456789", SimNumber = "1123456789" };
        await _helper.DbContext.Sims.AddAsync(sim);
        await _helper.DbContext.SaveChangesAsync();
        string addDateTime = sim.LastUpdate;
        sim.AssetTag = "AssetTag";

        await Task.Delay(1000); // make sure timestamps differ
        _helper.DbContext.Sims.Update(sim);
        await _helper.DbContext.SaveChangesAsync();

        Assert.NotEqual(addDateTime, sim.LastUpdate);
    }
}
