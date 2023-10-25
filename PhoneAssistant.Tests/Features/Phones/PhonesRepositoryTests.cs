using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Phones;

using Xunit;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class PhonesRepositoryTests : DbTestHelper
{
    [Fact]
    public async Task RemoveSimFromPhone_WithNullPhone_ThrowsException()
    {
        v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
#pragma warning disable CS8600, CS8604 // Converting null literal or possible null value to non-nullable type.
        v1Phone phone = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.RemoveSimFromPhone(phone));
#pragma warning restore CS8600, CS8604 // Converting null literal or possible null value to non-nullable type.
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task RemoveSimFromPhone_WithNullOrEmptyPhoneNumber_ThrowsException(string? phoneNumber)
    {
        v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
        v1Phone phone = new() 
        { 
            PhoneNumber = phoneNumber,
            Imei = "imei", 
            Model = "model", 
            NorR = "norr", 
            OEM = "oem", 
            Status = "status"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => repository.RemoveSimFromPhone(phone));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task RemoveSimFromPhone_WithNullOrEmptySimNumber_ThrowsException(string? simNumber)
    {
        v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
        v1Phone phone = new()
        {
            SimNumber = simNumber,
            Imei = "imei",
            PhoneNumber = "phone number",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => repository.RemoveSimFromPhone(phone));
    }

    [Fact]
    public async Task RemoveSimFromPhone_WithMissingPhone_ThrowsException()
    {
        using v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.RemoveSimFromPhone(phone));
    }

    [Fact]
    public async Task RemoveSimFromPhone_WithExistingSim_ThrowsException()
    {
        using v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
        v1Phone phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone number",
            SimNumber = "sim number",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };
        await helper.DbContext.Phones.AddAsync(phone);
        v1Sim sim = new v1Sim() { PhoneNumber = "phone number", SimNumber = "sim number" };
        await helper.DbContext.Sims.AddAsync(sim);
        await helper.DbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.RemoveSimFromPhone(phone));
    }

    [Fact]
    public async Task RemoveSimFromPhone_WithNewSim_Succeeds()
    {
        using v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
        const string PHONE_NUMBER = "phone number";
        const string SIM_NUMBER = "sim number";
        v1Phone? phone = new()
        {
            Imei = "imei",
            PhoneNumber = PHONE_NUMBER,
            SimNumber = SIM_NUMBER,
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };
        await helper.DbContext.Phones.AddAsync(phone);
        await helper.DbContext.SaveChangesAsync();

        string lastUpdate = await repository.RemoveSimFromPhone(phone);

        v1Sim? sim = await helper.DbContext.Sims.FindAsync(PHONE_NUMBER);
        Assert.NotNull(sim);
        Assert.Equal(PHONE_NUMBER, sim.PhoneNumber);
        Assert.Equal(SIM_NUMBER, sim.SimNumber);
        Assert.Equal("In Stock", sim.Status);

        phone = await helper.DbContext.Phones.FindAsync("imei");
        Assert.NotNull(phone);
        Assert.Null(phone.PhoneNumber);
        Assert.Null(phone.SimNumber);
        Assert.Matches("[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", lastUpdate);
    }

    [Fact]
    public async Task UpdateAsync_WithNullPhone_ThrowsException()
    {
        v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
#pragma warning disable CS8600, CS8604 // Converting null literal or possible null value to non-nullable type.
        v1Phone phone = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateAsync(phone));
#pragma warning restore CS8600, CS8604 // Possible null reference argument.
    }

    [Fact]
    public async Task UpdateAsync_WithPhoneNotFound_ThrowsException()
    {
        using v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
        v1Phone? expected = new()
        {
            Imei = "imei",
            PhoneNumber = "phone2",
            SimNumber = "sim2",
            Model = "model2",
            NorR = "norr2",
            OEM = "oem2",
            Status = "status2"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => repository.UpdateAsync(expected));    }

    [Fact]
    public async Task UpdateAsync_WithPhoneFound_Succeeds()
    {
        using v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
        v1Phone? phone = new()
        {
            Imei = "imei",
            PhoneNumber = "phone",
            SimNumber = "sim",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };
        await helper.DbContext.Phones.AddAsync(phone);
        await helper.DbContext.SaveChangesAsync();
        v1Phone? expected = new()
        {
            Imei = "imei",
            PhoneNumber = "phone2",
            SimNumber = "sim2",
            Model = "model2",
            NorR = "norr2",
            OEM = "oem2",
            Status = "status2"
        };

        string lastUpdate = await repository.UpdateAsync(expected);

        v1Phone? actual = await helper.DbContext.Phones.FindAsync("imei");
        Assert.NotNull(actual);                
        Assert.Equal(expected.AssetTag, actual.AssetTag);
        Assert.Equal(expected.FormerUser, actual.FormerUser);
        Assert.Equal(expected.Imei, actual.Imei);
        Assert.Equal(expected.Model, actual.Model);
        Assert.Equal(expected.NewUser, actual.NewUser);
        Assert.Equal(expected.NorR, actual.NorR);
        Assert.Equal(expected.Notes, actual.Notes);
        Assert.Equal(expected.OEM, actual.OEM);
        Assert.Equal(expected.PhoneNumber, actual.PhoneNumber);
        Assert.Equal(expected.SimNumber, actual.SimNumber);
        Assert.Equal(expected.SR, actual.SR);
        Assert.Equal(expected.Status, actual.Status);
        Assert.Matches("[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", lastUpdate);
    }

    [Fact]
    public async Task UpdateKeyAsync_WithNullOldImei_ThrowsException()
    {
        v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
#pragma warning disable CS8625 // Converting null literal or possible null value to non-nullable type.
        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateKeyAsync(null,"new"));
#pragma warning restore CS8625 // Possible null reference argument.
    }

    [Fact]
    public async Task UpdateKeyAsync_WithNullNewImei_ThrowsException()
    {
        v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
#pragma warning disable CS8625 // Converting null literal or possible null value to non-nullable type.
        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateKeyAsync("old", null));
#pragma warning restore CS8625 // Possible null reference argument.
    }

    [Fact]
    public async Task UpdateKeyAsync_WithPhoneNotFound_ThrowsException()
    {
        using v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
        v1Phone? expected = new()
        {
            Imei = "imei",
            PhoneNumber = "phone2",
            SimNumber = "sim2",
            Model = "model2",
            NorR = "norr2",
            OEM = "oem2",
            Status = "status2"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => repository.UpdateAsync(expected));
    }

    [Fact]
    public async Task UpdateKeyAsync_WithPhoneFound_Succeeds()
    {
        using v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
        const string OLD_IMEI = "old IMEI";
        v1Phone? phone = new()
        {
            Imei = OLD_IMEI,
            PhoneNumber = "phone",
            SimNumber = "sim",
            Model = "model",
            NorR = "norr",
            OEM = "oem",
            Status = "status"
        };
        await helper.DbContext.Phones.AddAsync(phone);
        await helper.DbContext.SaveChangesAsync();
        const string NEW_IMEI = "new IMEI";

        string lastUpdate = await repository.UpdateKeyAsync(phone.Imei, NEW_IMEI);

        v1Phone? removed = await helper.DbContext.Phones.FindAsync(OLD_IMEI);
        Assert.Null(removed);
        v1Phone? actual = await helper.DbContext.Phones.FindAsync(NEW_IMEI);
        Assert.NotNull(actual);
        Assert.Equal(NEW_IMEI, actual.Imei);
        Assert.Matches("[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", lastUpdate);
    }
}
