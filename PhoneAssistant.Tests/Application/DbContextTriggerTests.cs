using PhoneAssistant.WPF.Application.Entities;

using System.Threading.Tasks;

namespace PhoneAssistant.Tests.Application;
public class DbContextTriggerTests() : DbTestHelper
{
    readonly DbTestHelper _helper = new();

    [Test]
    public async Task AddPhone_ShouldSetLastUpdate()
    {
        Phone phone = new() { Condition = "R", Imei = "imei", Model = "model", OEM = OEMs.Nokia, Status = "Production" };
        await Assert.That(phone.LastUpdate).IsEmpty();

        await _helper.DbContext.Phones.AddAsync(phone);
        await _helper.DbContext.SaveChangesAsync();

        await Assert.That(phone.LastUpdate).IsNotEmpty();
    }

    [Test]
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

        await Assert.That(phone.LastUpdate).IsNotEqualTo(addDateTime);
    }
}
