using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Sims;

using Xunit;

namespace PhoneAssistant.Tests.Features.Sims;
public sealed class SimsRepositoryTests
{
    [Fact]
    public async Task MoveSimToPhone_WithNullPhoneNumber_ThrowsException()
    {
        v1DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        using v1PhoneAssistantDbContext dbContext = new(helper.Options!);
        await dbContext.Database.EnsureCreatedAsync();
        SimsRepository repository = new(dbContext);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.MoveSimToPhone(null,"ignored"));
    }

    [Fact]
    public async Task MoveSimToPhone_WithNotFoundSim_ThrowsException()
    {
        v1DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        using v1PhoneAssistantDbContext dbContext = new(helper.Options!);
        await dbContext.Database.EnsureCreatedAsync();
        SimsRepository repository = new (dbContext);

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.MoveSimToPhone("1","ignored"));
    }

    [Fact]
    public async Task MoveSimToPhone_WithNotFoundPhone_ThrowsException()
    {
        v1DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        using v1PhoneAssistantDbContext dbContext = new(helper.Options!);
        await dbContext.Database.EnsureCreatedAsync();
        SimsRepository repository = new(dbContext);
        v1Sim sim = new v1Sim() { PhoneNumber = "sim1", SimNumber = "simnumber" };
        await dbContext.Sims.AddAsync(sim);
        await dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.MoveSimToPhone("sim1", "imei1"));
    }

    [Fact]
    public async Task MoveSimToPhone_WithFoundSimAndPhone_Succeeds()
    {
        v1DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        using v1PhoneAssistantDbContext dbContext = new(helper.Options!);
        await dbContext.Database.EnsureCreatedAsync();
        SimsRepository repository = new(dbContext);
        v1Sim sim = new v1Sim() { PhoneNumber = "sim1", SimNumber = "simnumber" };
        await dbContext.Sims.AddAsync(sim);
        v1Phone phone = new() { Imei = "imei1" };
        await dbContext.Phones.AddAsync(phone);
        await dbContext.SaveChangesAsync();

        await repository.MoveSimToPhone(sim.PhoneNumber, phone.Imei);

        v1Phone? actualPhone = await dbContext.Phones.FindAsync(phone.Imei);
        Assert.NotNull(actualPhone);
        Assert.Equal(sim.PhoneNumber, actualPhone.PhoneNumber);
        Assert.Equal(sim.SimNumber, actualPhone.SimNumber);
        v1Sim? actualSim = await dbContext.Sims.FindAsync(sim.PhoneNumber);
        Assert.Null(actualSim);
    }
}
