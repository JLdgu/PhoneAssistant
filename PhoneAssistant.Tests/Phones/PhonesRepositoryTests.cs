using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.Tests.Phones;

[TestClass]
public sealed class PhonesRepositoryTests : DbTestBase
{
    [TestMethod]
    public async Task SearchAsync_ReturnsEmptyList_WhenIMEINotFound()
    {
        using PhoneAssistantDbContext dbContext = new PhoneAssistantDbContext(Options);

        // Arrange
        PhoneEntity[] testdata = new PhoneEntity[]
        {
            new PhoneEntity() {Id = 1, IMEI = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
            new PhoneEntity() {Id = 2, IMEI = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00017", Note = "Replacement"},
            new PhoneEntity() {Id = 3, IMEI = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 4, IMEI = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 5, IMEI = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 6, IMEI = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00016", Note = "Replacement"}
        };
        dbContext.MobilePhones.AddRange(testdata);
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
        using PhoneAssistantDbContext dbContext = new PhoneAssistantDbContext(Options);

        // Arrange
        PhoneEntity[] testdata = new PhoneEntity[]
        {
            new PhoneEntity() {Id = 1, IMEI = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
            new PhoneEntity() {Id = 2, IMEI = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00729", Note = "Replacement"},
            new PhoneEntity() {Id = 3, IMEI = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 4, IMEI = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 5, IMEI = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 6, IMEI = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00768", Note = "Replacement"}
        };
        dbContext.MobilePhones.AddRange(testdata);
        dbContext.SaveChanges();

        var repository = new PhonesRepository(dbContext);

        // Act
        var actual = await repository.SearchAsync("729");
        var actualPhone = actual.First();

        // Assert
        Assert.AreEqual(2, actual.Count());
    }
}
