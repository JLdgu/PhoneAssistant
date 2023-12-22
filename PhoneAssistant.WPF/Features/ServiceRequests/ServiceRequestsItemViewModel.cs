using System.ComponentModel.DataAnnotations;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.ServiceRequests
{
    public sealed partial class ServiceRequestsItemViewModel : ObservableValidator
    {
        private ServiceRequest _srPreAmendments;
        private readonly IServiceRequestsRepository _srRepository;

        public ServiceRequestsItemViewModel(IServiceRequestsRepository serviceRequestsRepository)
        {
            _srRepository = serviceRequestsRepository;
            SR = new() { NewUser = "" };
        }

        [ObservableProperty]
        private ServiceRequest _sR;

        private bool ValidChanges()
        {
            CanCancelSRChanges = true;
            CanCreateNewSR = false;

            if (HasErrors || ServiceRequestNumber == 0 || NewUser == string.Empty)
                CanSaveSRChanges = false;
            else
                CanSaveSRChanges = true;

            return true;
        }
        private void SRChanged(ServiceRequest value)
        {
            _srPreAmendments = new()
            {
                Id = value.Id,
                ServiceRequestNumber = value.ServiceRequestNumber,
                NewUser = value.NewUser,
                DespatchDetails = value.DespatchDetails
            };
            ServiceRequestNumber = value.ServiceRequestNumber;
            NewUser = value.NewUser;
            DespatchDetails = value.DespatchDetails;

            if (value.Id != 0)
                CanCreateNewSR = true;

            ClearErrors();
        }

        partial void OnSRChanged(ServiceRequest value)
        {
            SRChanged(value);
        }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required]
        [Range(150000,999999, ErrorMessage = "Value must be between {1} and {2}.")]
        private int _serviceRequestNumber;

        partial void OnServiceRequestNumberChanged(int value)
        {
            if (value == SR.ServiceRequestNumber) return;

            if (ValidChanges())
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

            if (ValidChanges())
                SR.NewUser = value;
        }

        [ObservableProperty]
        private string? _despatchDetails;

        partial void OnDespatchDetailsChanged(string? value)
        {            
            if (value == SR.DespatchDetails) return;

            if (ValidChanges())
                SR.DespatchDetails = value;
        }

        [ObservableProperty]
        private bool canSaveSRChanges;

        [RelayCommand]
        private async Task SaveSRChangesAsync()
        {
            await _srRepository.UpdateAsync(SR);

            CanCancelSRChanges = false;
            CanSaveSRChanges = false;
            CanCreateNewSR = true;
        }

        [ObservableProperty]
        private bool canCancelSRChanges;

        [RelayCommand]
        private void CancelSRChanges()
        {
            SRChanged(_srPreAmendments);

            CanSaveSRChanges = false;
            CanCancelSRChanges = false;
        }

        [ObservableProperty]
        private bool canCreateNewSR;

        [RelayCommand]
        private void CreateNewSR()
        {

        }

    }
}