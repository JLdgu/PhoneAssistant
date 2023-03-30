using System.ComponentModel.DataAnnotations;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.ServiceRequests
{
    public sealed partial class ServiceRequestsItemViewModel : ObservableValidator
    {
        public ServiceRequestsItemViewModel()
        {
            SR = new() { Id = 0, ServiceRequestNumber = 0, NewUser = "", DespatchDetails = null };
            ClearErrors();
        }

        public ServiceRequestsItemViewModel(ServiceRequest serviceRequest)
        {
            SR = serviceRequest;
        }

        [ObservableProperty]
        private ServiceRequest _sR;

        private bool _srUnchanged
        {
            get 
            { 
                if (ServiceRequestNumber != SR.ServiceRequestNumber)
                    return false;
                if (NewUser != SR.NewUser)
                    return false;
                if (DespatchDetails != SR.DespatchDetails)
                    return false;
                return true;
            }
        }

        partial void OnSRChanged(ServiceRequest value)
        {
            ServiceRequestNumber = value.ServiceRequestNumber;
            NewUser = value.NewUser;
            DespatchDetails = value.DespatchDetails;

            if (value.Id != 0)
                CanCreateNewSR = true;
        }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required]
        [Range(150000,999999, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        private int _serviceRequestNumber;

        partial void OnServiceRequestNumberChanged(int value)
        {
            if (value == SR.ServiceRequestNumber) return;

            if (_srUnchanged)
            {
                CanCancelSRChanges = false;
            }
            else
            {
                CanCancelSRChanges = true;
                CanCreateNewSR = false;
            }

            if (HasErrors)
            {
                CanSaveSRChanges = false;
                return;
            }

            CanSaveSRChanges = true;
            SR.ServiceRequestNumber = value;
        }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required]
        [MinLength(2)]
        private string _newUser;

        partial void OnNewUserChanged(string value)
        {
            if (value == SR.NewUser) return;

            if (_srUnchanged)
            {
                CanCancelSRChanges = false;
            }
            else
            {
                CanCancelSRChanges = true;
                CanCreateNewSR = false;
            }

            if (HasErrors)
            {
                CanSaveSRChanges = false;
                return;
            }

            CanSaveSRChanges = true;
            SR.NewUser = value;
        }

        [ObservableProperty]
        private string? _despatchDetails;

        partial void OnDespatchDetailsChanged(string? value)
        {            
            if (value == SR.DespatchDetails) return;

            if (_srUnchanged)
            {
                CanCancelSRChanges = false;
            }
            else
            {
                CanCancelSRChanges = true;
                CanCreateNewSR = false;
            }

            if (HasErrors)
            {
                CanSaveSRChanges = false;
                return;
            }

            CanSaveSRChanges = true;
            SR.DespatchDetails = value;
        }

        public bool CanSaveSRChanges { get; private set; }

        [RelayCommand]
        private void SaveSRChanges()
        {

        }

        public bool CanCancelSRChanges { get; private set; }

        [RelayCommand]
        private void CancelSRChanges()
        {

        }

        public bool CanCreateNewSR { get; private set; }

        [RelayCommand]
        private void CreateNewSR()
        {

        }

    }
}