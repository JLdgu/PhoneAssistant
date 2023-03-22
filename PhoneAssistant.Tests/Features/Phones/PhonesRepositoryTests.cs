using Microsoft.Data.Sqlite;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Models;

namespace PhoneAssistant.Tests.Features.Phones;

[TestClass]
public sealed class PhonesRepositoryTests : DbTestHelper
{
    #region Search

    [TestMethod]
    public async Task SearchAsync_ReturnsEmptyList_WhenIMEINotFound()
    {
        DbTestHelper helper = new();
        using SqliteConnection Connection = helper.CreateConnection();
        using PhoneAssistantDbContext dbContext = new PhoneAssistantDbContext(helper.Options!);
        dbContext.Database.EnsureCreated();

        PhoneEntity[] testdata = new PhoneEntity[]
        {
            new PhoneEntity() {Id = 1, IMEI = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
            new PhoneEntity() {Id = 2, IMEI = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00017", Note = "Replacement"},
            new PhoneEntity() {Id = 3, IMEI = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 4, IMEI = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 5, IMEI = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 6, IMEI = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00016", Note = "Replacement"}
        };
        dbContext.Phones.AddRange(testdata);
        dbContext.SaveChanges();

        var repository = new PhonesRepository(dbContext);

        // Act
        var actual = await repository.SearchAsync("not found");

        // Assert
        Assert.AreEqual(0, actual.Count());
    }

    [TestMethod]
    public async Task SearchAsync_ReturnsList_WhenSearchTermFound()
    {
        DbTestHelper helper = new();
        using SqliteConnection Connection = helper.CreateConnection();
        using PhoneAssistantDbContext dbContext = new PhoneAssistantDbContext(helper.Options!);
        dbContext.Database.EnsureCreated();

        PhoneEntity[] testdata = new PhoneEntity[]
        {
            new PhoneEntity() {Id = 1, IMEI = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
            new PhoneEntity() {Id = 2, IMEI = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00729", Note = "Replacement"},
            new PhoneEntity() {Id = 3, IMEI = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 4, IMEI = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 5, IMEI = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 6, IMEI = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00768", Note = "Replacement"}
        };
        dbContext.Phones.AddRange(testdata);
        dbContext.SaveChanges();

        var repository = new PhonesRepository(dbContext);

        var actual = await repository.SearchAsync("729");

        var actualPhone = actual.First();
        Assert.AreEqual(2, actual.Count());
    }
}
#endregion

[TestClass]
public sealed class PhonesRepositoryUpdateTests : DbTestHelper
{
    [TestMethod()]
    public async Task UpdateAsync()
    {
        Phone phoneToUpdate = new(
            id: 1,
            iMEI: "A",
            formerUser: "A",
            wiped: true,
            status: "A",
            oEM: "A",
            assetTag: "A",
            note: "A" );

        DbTestHelper helper = new();
        using SqliteConnection Connection = helper.CreateConnection();

        using PhoneAssistantDbContext dbContext = new PhoneAssistantDbContext(helper.Options!);
        dbContext.Database.EnsureCreated();
        dbContext.Phones.Add(phoneToUpdate);
        dbContext.SaveChanges();

        var repository = new PhonesRepository(dbContext);
        phoneToUpdate.IMEI = "B";
        phoneToUpdate.FormerUser = "B";
        phoneToUpdate.Wiped = false;
        phoneToUpdate.Status = "B";
        phoneToUpdate.OEM = "B";
        phoneToUpdate.AssetTag = "B";
        phoneToUpdate.Note = "B";

        await repository.UpdateAsync(phoneToUpdate);

        PhoneEntity actual = dbContext.Phones.Where(mp => mp.Id == 1).First();
        //Assert.AreEqual(phoneToUpdate, actual);
        Assert.AreEqual(phoneToUpdate.IMEI, actual.IMEI);
        Assert.AreEqual(phoneToUpdate.FormerUser, actual.FormerUser);
        Assert.IsFalse(actual.Wiped);
        Assert.AreEqual(phoneToUpdate.Status, actual.Status);
        Assert.AreEqual(phoneToUpdate.OEM, actual.OEM);
        Assert.AreEqual(phoneToUpdate.AssetTag, actual.AssetTag);
    }
}
