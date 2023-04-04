using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;

namespace PhoneAssistant.Tests.Application;

[TestClass]
public class ScriptDBContext
{
    [TestMethod]
    [TestCategory("SQLScript")]
    public void GenerateSQLScript()
    {
        DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        using PhoneAssistantDbContext dbContext = new(helper.Options!);

        dbContext.Database.EnsureCreated();

        string sql = dbContext.Database.GenerateCreateScript();

        File.WriteAllText("c:/temp/DbCreate.sql", sql);       
    }

    [TestMethod]
    public void DbSchemaTest()
    {
        DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        //using SqliteConnection connection = helper.CreateConnection(@"DataSource=c:\dev\sqlite\schema.db;");
        using PhoneAssistantDbContext dbContext = new(helper.Options!);
        dbContext.Database.EnsureCreated();

        Phone phoneStandAlone = new() { Imei = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null };        
        Link linkPhone = new(); 
        linkPhone.Phone = phoneStandAlone;
        dbContext.Links.Add(linkPhone);
        dbContext.SaveChanges();
        Assert.AreEqual(1,phoneStandAlone.Id);
        Assert.AreEqual(1,linkPhone.Id);
        Assert.IsNull(linkPhone.Sim);
        Assert.IsNull(linkPhone.ServiceRequest);

        Phone phoneStandAlone2 = new() { Imei = "355808981132845", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null };        
        Link linkPhone2 = new(); 
        linkPhone2.Phone = phoneStandAlone2;
        dbContext.Links.Add(linkPhone2);
        dbContext.SaveChanges();
        Assert.AreEqual(2,phoneStandAlone2.Id);
        Assert.AreEqual(2,linkPhone2.Id);
        Assert.IsNull(linkPhone2.Sim);
        Assert.IsNull(linkPhone2.ServiceRequest);

        Sim simStandAlone = new() { PhoneNumber = "07967691765", SimNumber = "8944125605559639023" };
        Link linkSim = new();        
        linkSim.Sim = simStandAlone;
        dbContext.Links.Add(linkSim);
        dbContext.SaveChanges();
        Assert.AreEqual(1,simStandAlone.Id);
        Assert.AreEqual(3,linkSim.Id);
        Assert.IsNull(linkSim.Phone);
        Assert.IsNull(linkSim.ServiceRequest);

        //     new Phone() { Id = 3, Imei = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null }
        //     new Phone() { Id = 4, Imei = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null }
        //     new Phone() { Id = 5, Imei = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null }
        //     new Phone() { Id = 6, Imei = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00016", Note = "Replacement" }

        //     new Sim() { Id = 31, PhoneNumber = "07967691763", SimNumber = "8944125605559639323" },
        //     new Sim() { Id = 41, PhoneNumber = "07967691762", SimNumber = "8944125605559639453" },
        //     new Sim() { Id = 51, PhoneNumber = "07967691761", SimNumber = "8944125605559639893" },
        //     new Sim() { Id = 61, PhoneNumber = "07967691760", SimNumber = "8944125605559639999" }

        // ServiceRequest standaloneSR = new() { Id = 1, ServiceRequestNumber = 101, NewUser = "Test User 101" };
        //     new ServiceRequest() { Id = 2, ServiceRequestNumber = 202, NewUser = "Test User 202", DespatchDetails = "DD202" },
        //     new ServiceRequest() { Id = 3, ServiceRequestNumber = 303, NewUser = "Test User 303" },
        //     new ServiceRequest() { Id = 4, ServiceRequestNumber = 404, NewUser = "Test User 404", DespatchDetails = "DD404" },
        //     new ServiceRequest() { Id = 5, ServiceRequestNumber = 505, NewUser = "Test User 505" }
        
    }

}