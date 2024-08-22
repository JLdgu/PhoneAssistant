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
}
