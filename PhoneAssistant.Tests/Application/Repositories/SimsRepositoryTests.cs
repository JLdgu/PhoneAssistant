using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

using Xunit;

namespace PhoneAssistant.Tests.Application.Repositories;
public sealed class SimsRepositoryTests
{
    readonly DbTestHelper _helper = new();
    readonly SimsRepository _repository;

    public SimsRepositoryTests()
    {
        _repository = new(_helper.DbContext);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetLastUpdate()
    {
        Sim actual = new() { PhoneNumber = "phonenumber", SimNumber = "sim number" };
        string minLastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        await _repository.CreateAsync(actual);
        string maxLastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        Assert.InRange(actual.LastUpdate, minLastUpdate, maxLastUpdate);
    }

    [Fact]
    public async Task DeleteSIM_ShouldReturnNull_WhenSIMDoesNotExist()
    {
        string? actual = await _repository.DeleteSIMAsync("DoesNotExist");

        Assert.Null(actual);
    }

    [Fact]
    public async Task DeleteSIM_ShouldDeleteSIMAndReturnSIMNumber_WhenSIMDoesExist()
    {
        _helper.DbContext.Sims.Add(new Sim() { PhoneNumber = "phonenumber", SimNumber = "sim number" });
        await _helper.DbContext.SaveChangesAsync();

        string? actual = await _repository.DeleteSIMAsync("phonenumber");

        Assert.NotNull(actual);
        Assert.Equal("sim number", actual);

        Sim? actualSim = await _helper.DbContext.Sims.FindAsync("phoneNumber");
        Assert.Null(actualSim);
    }

    [Fact]
    public async Task GetSimNumber_ShouldReturnNull_WhenSIMDoesNotExist()
    {
        string? actual = await _repository.GetSIMNumberAsync("DoesNotExist");

        Assert.Null(actual);
    }

    [Fact]
    public async Task GetSimNumber_ShouldReturnSIMNumber_WhenSIMDoesExist()
    {
        _helper.DbContext.Sims.Add(new Sim() { PhoneNumber = "phonenumber", SimNumber = "sim number" });
        await _helper.DbContext.SaveChangesAsync();

        string? actual = await _repository.GetSIMNumberAsync("phonenumber");

        Assert.Equal("sim number", actual);
    }
}
