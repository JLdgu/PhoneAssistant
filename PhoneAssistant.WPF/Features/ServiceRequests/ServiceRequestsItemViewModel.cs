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
            ServiceRequest = serviceRequest;
            ServiceRequestNumber = ServiceRequest.ServiceRequestNumber;
            NewUser = ServiceRequest.NewUser;
        }

        public int SRWidth { get; set; }

        [ObservableProperty]
        private ServiceRequest _serviceRequest;
        //public ServiceRequest ServiceRequest
        //{
        //    get => _serviceRequest;
        //    set
        //    {
        //        _serviceRequest = value;
        //    }
        //}

        partial void OnServiceRequestChanged(ServiceRequest value)
        {
            ServiceRequestNumber = value.ServiceRequestNumber;
            NewUser = value.NewUser;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ServiceRequest))]
        private int _serviceRequestNumber;

        partial void OnServiceRequestNumberChanged(int value)
        {
            ServiceRequest.ServiceRequestNumber = value;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ServiceRequest))]
        private string _newUser;

        partial void OnNewUserChanged(string value)
        {
            ServiceRequest.NewUser = value;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ServiceRequest))]
        private string? _despatchDetails;

        partial void OnDespatchDetailsChanged(string? value)
        {
            ServiceRequest.DespatchDetails = value;
        }

        [RelayCommand]
        private void UpdateServiceRequest()
        {

        }
    }
}
