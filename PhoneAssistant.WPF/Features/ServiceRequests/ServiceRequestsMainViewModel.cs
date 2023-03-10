using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.ServiceRequests;

public sealed class ServiceRequestsMainViewModel : IServiceRequestsMainViewModel
{
    //public List<Employer> Employers { get; set; } = new List<Employer>();

    //public List<string> Sectors { get; set; } = new List<string>();

    public Task LoadAsync()
    {
        //Employers = new List<Employer>()
        //{
        //    new Employer() { SName = "ABC.com", Sector = "Public" },
        //    new Employer() { SName = "DEF", Sector = "Private" },
        //    new Employer() { SName = "HIJ", Sector = "Public" },
        //    new Employer() { SName = "XYZ", Sector = "Private" }
        //};

        ////Sectors = new List<string>();
        //Sectors.Add("Public");
        //Sectors.Add("Private");

        return Task.CompletedTask;
    }
}

//public class Employer
//{
//    public string SName { get; set; }
//    public string Sector { get; set; }
//}
