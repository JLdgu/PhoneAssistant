using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Models;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed class PhonesRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public PhonesRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Phone>> AllAsync()
    {
        List<PhoneEntity> MobilePhones = await _dbContext.MobilePhones.ToListAsync();
        List<Phone> phones = new List<Phone>();
        foreach (PhoneEntity mobile in MobilePhones)
        {
            phones.Add(new Phone
            {
                Id = mobile.Id,
                IMEI = mobile.IMEI,
                FormerUser = mobile.FormerUser,
                Wiped = mobile.Wiped,
                Status = mobile.Status,
                OEM = mobile.OEM
            });
        }
        return phones;
    }

    public void SaveChanges(Phone changedPhone)
    {
        PhoneEntity mobile = new PhoneEntity
        {
            Id = changedPhone.Id,
            IMEI = changedPhone.IMEI,
            FormerUser = changedPhone.FormerUser,
            Wiped = changedPhone.Wiped,
            Status = changedPhone.Status,
            OEM = changedPhone.OEM
        };
        //_dbContext.MobilePhones.Update(mobile);
        //_dbContext.SaveChanges();
    }

    public static IEnumerable<Phone> AllPhones()
    {
        return new List<Phone>()
        {
            new Phone () { IMEI = "353427866717729", FormerUser = "Rehana Kausar", Wiped= true, Status="In Stock", OEM="Samsung", AssetTag=null, Note=null, NewUser=null, DespatchDetails=null},
            new Phone () { IMEI = "355808981132845", FormerUser = null,Wiped=true, Status="Production", OEM="Samsung", AssetTag="MP00017", Note="Replacement", NewUser="Claire Turner", DespatchDetails="DF971905874GB" },
            new Phone () { IMEI = "355808983976082" ,FormerUser = "Charlie Baker"   ,Wiped=true,  Status="In Stock", OEM="Samsung", AssetTag=null, Note=null, NewUser=null, DespatchDetails=null},
            new Phone () { IMEI = "355808981101899" ,FormerUser = "Olatunji Okedeyi", Wiped=false,  Status="In Stock", OEM="Samsung", AssetTag=null, Note=null, NewUser=null, DespatchDetails=null},
            new Phone () { IMEI = "353427861419768" ,FormerUser = "James Tisshaw" ,Wiped=false,  Status="In Stock", OEM="Samsung", AssetTag=null, Note=null, NewUser=null, DespatchDetails=null},
            new Phone () { IMEI = "351554742085336" ,FormerUser = null, Wiped=true, Status="Production", OEM="Samsung" , AssetTag="MP00016", Note="Replacement", NewUser="Amanda Paterson", DespatchDetails="DF971905928GB"}
        };
    }
}