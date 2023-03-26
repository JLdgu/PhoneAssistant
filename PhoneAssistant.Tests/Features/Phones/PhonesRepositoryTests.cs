using Microsoft.Data.Sqlite;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Features.Phones;

namespace PhoneAssistant.Tests.Features.Phones;

[TestClass]
public sealed class PhonesRepositoryTests : DbTestHelper
{
    #region Search

    [TestMethod]
    public async Task SearchAsync_ReturnsEmptyList_WhenIMEINotFound()
    {
        DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        using PhoneAssistantDbContext dbContext = new(helper.Options!);
        dbContext.Database.EnsureCreated();

        Phone[] testdata = new Phone[]
        {
            new Phone() {Id = 1, Imei = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
            new Phone() {Id = 2, Imei = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00017", Note = "Replacement"},
            new Phone() {Id = 3, Imei = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new Phone() {Id = 4, Imei = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new Phone() {Id = 5, Imei = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new Phone() {Id = 6, Imei = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00016", Note = "Replacement"}
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
        using SqliteConnection connection = helper.CreateConnection();
        using PhoneAssistantDbContext dbContext = new(helper.Options!);
        dbContext.Database.EnsureCreated();

        Phone[] testdata = new Phone[]
        {
            new Phone() {Id = 1, Imei = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
            new Phone() {Id = 2, Imei = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00729", Note = "Replacement"},
            new Phone() {Id = 3, Imei = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new Phone() {Id = 4, Imei = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new Phone() {Id = 5, Imei = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new Phone() {Id = 6, Imei = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00768", Note = "Replacement"}
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