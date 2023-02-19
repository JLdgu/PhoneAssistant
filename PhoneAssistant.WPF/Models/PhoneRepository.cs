using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Models;

public sealed class PhoneRepository
{
    public async Task<IEnumerable<Phone>> AllAsync()
    {
        await Task.CompletedTask;
        return AllPhones();
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