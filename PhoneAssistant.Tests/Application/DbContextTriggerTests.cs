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
}
