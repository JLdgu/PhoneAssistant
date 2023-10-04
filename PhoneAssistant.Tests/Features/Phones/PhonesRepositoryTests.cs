using Microsoft.Data.Sqlite;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.Sims;
using Xunit;
using System.Numerics;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class PhonesRepositoryTests : DbTestHelper
{
    [Fact]
    public async Task RemoveSimFromPhone_WithNullPhone_ThrowsException()
    {
        v1DbTestHelper helper = new();
        PhonesRepository repository = new(helper.DbContext);
        v1Phone phone = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.RemoveSimFromPhone(phone));
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
        await helper.DbContext.SaveChangesAsync();

        await repository.RemoveSimFromPhone(phone);

        v1Sim actual = await helper.DbContext.Sims.FindAsync(phone.PhoneNumber);
        Assert.NotNull(actual);
        Assert.Equal(phone.PhoneNumber, actual.PhoneNumber);
        Assert.Equal(phone.SimNumber, actual.SimNumber);
        Assert.Equal("In Stock", actual.Status);
    }

    #region Search
    //[TestMethod]
    //public async Task SearchAsync_ReturnsEmptyList_WhenIMEINotFound()
    //{
    //    DbTestHelper helper = new();
    //    using SqliteConnection connection = helper.CreateConnection();
    //    using PhoneAssistantDbContext dbContext = new(helper.Options!);
    //    dbContext.Database.EnsureCreated();

    //    Phone[] testdata = new Phone[]
    //    {
    //        new Phone() {Id = 1, Imei = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
    //        new Phone() {Id = 2, Imei = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00017", Note = "Replacement"},
    //        new Phone() {Id = 3, Imei = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
    //        new Phone() {Id = 4, Imei = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
    //        new Phone() {Id = 5, Imei = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
    //        new Phone() {Id = 6, Imei = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00016", Note = "Replacement"}
    //    };
    //    dbContext.Phones.AddRange(testdata);
    //    dbContext.SaveChanges();

    //    var repository = new PhonesRepository(dbContext);

    //    // Act
    //    var actual = await repository.SearchAsync("not found");

    //    // Assert
    //    Assert.AreEqual(0, actual.Count());
    //}

    //[TestMethod]
    //public async Task SearchAsync_ReturnsList_WhenSearchTermFound()
    //{
    //    DbTestHelper helper = new();
    //    using SqliteConnection connection = helper.CreateConnection();
    //    using PhoneAssistantDbContext dbContext = new(helper.Options!);
    //    dbContext.Database.EnsureCreated();

    //    Phone[] testdata = new Phone[]
    //    {
    //        new Phone() {Id = 1, Imei = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
    //        new Phone() {Id = 2, Imei = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00729", Note = "Replacement"},
    //        new Phone() {Id = 3, Imei = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
    //        new Phone() {Id = 4, Imei = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
    //        new Phone() {Id = 5, Imei = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
    //        new Phone() {Id = 6, Imei = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00768", Note = "Replacement"}
    //    };
    //    dbContext.Phones.AddRange(testdata);
    //    dbContext.SaveChanges();

    //    var repository = new PhonesRepository(dbContext);

    //    var actual = await repository.SearchAsync("729");

    //    var actualPhone = actual.First();
    //    Assert.AreEqual(2, actual.Count());
    //}
}
#endregion