using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;
public sealed class PhonesItemViewModel
{
    //public string Imei { get; set; }
    //public string? PhoneNumber { get; set; }
    //public string? SimNumber { get; set; }
    //public string? FormerUser { get; set; }
    //public  string NorR { get; set; }
    //public  string Status { get; set; }
    //public  string OEM { get; set; }
    //public  string Model { get; set; }
    //public int? SR { get; set; }
    //public string? AssetTag { get; set; }
    //public string? NewUser { get; set; }
    //public string? Notes { get; set; }
    //public string? LastUpdate { get; }
    public v1Phone Phone { get; }

    public PhonesItemViewModel(v1Phone phone)
    {
        //Imei = phone.Imei;
        //PhoneNumber = phone.PhoneNumber;
        //SimNumber = phone.SimNumber;
        //FormerUser = phone.FormerUser;
        //NorR = phone.NorR;
        //Status = phone.Status;
        //OEM = phone.OEM;
        //Model = phone.Model;
        //SR = phone.SR;
        //AssetTag = phone.AssetTag;
        //NewUser = phone.NewUser;
        //Notes = phone.Notes;
        //LastUpdate = phone.LastUpdate;
        Phone = phone;
    }

}
