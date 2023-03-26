using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.ServiceRequests
{
    public sealed partial class ServiceRequestsItemViewModel : ObservableObject
    {     

        public ServiceRequestsItemViewModel(ServiceRequest serviceRequest)
        {
            SR = serviceRequest;
        }

        public int SRWidth { get; set; }

        [ObservableProperty]
        private ServiceRequest _sR;

        partial void OnSRChanged(ServiceRequest value)
        {
            ServiceRequestNumber = value.ServiceRequestNumber;
            NewUser = value.NewUser;
            DespatchDetails = value.DespatchDetails;
        }

        [ObservableProperty]
        private int _serviceRequestNumber;

        partial void OnServiceRequestNumberChanged(int value)
        {
            SR.ServiceRequestNumber = value;
        }

        [ObservableProperty]
        private string _newUser;

        partial void OnNewUserChanged(string value)
        {
            SR.NewUser = value;
        }

        [ObservableProperty]
        private string? _despatchDetails;

        partial void OnDespatchDetailsChanged(string? value)
        {
            SR.DespatchDetails = value;
        }

        [RelayCommand]
        private void UpdateServiceRequest()
        {

        }

        //public bool SelectedPhoneHasUpdates { get; private set; }
    }
}
