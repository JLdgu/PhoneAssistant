using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.ServiceRequests;

public sealed partial class ServiceRequestsMainViewModel : ObservableObject, IServiceRequestsMainViewModel
{
    private readonly IServiceRequestsRepository _serviceRequestsRepository;

    public ServiceRequestsMainViewModel(IServiceRequestsRepository serviceRequestsRepository)
    {
        _serviceRequestsRepository = serviceRequestsRepository;
        ServiceRequest newSR = new () { Id = 0, ServiceRequestNumber = 0, NewUser = "" };

        ServiceRequestsItemViewModel = new ServiceRequestsItemViewModel(newSR);
    }

    public ServiceRequestsItemViewModel ServiceRequestsItemViewModel { get; }

    public ObservableCollection<ServiceRequest> ServiceRequests { get; } = new();

    [ObservableProperty]
    private ServiceRequest? _selectedServiceRequest;

    partial void OnSelectedServiceRequestChanged(ServiceRequest? value)
    {
        if (value is not null) ServiceRequestsItemViewModel.SR = value;
    }

    [ObservableProperty]
    private ListCollectionView _serviceRequestsView;

    public async Task LoadAsync()
    {
        if (ServiceRequests.Any()) return;

        var serviceRequests = await _serviceRequestsRepository.GetServiceRequestsAsync();
        if (serviceRequests == null)
        {
            throw new ArgumentNullException(nameof(serviceRequests));
        }

        foreach (var sr in serviceRequests)
        {
            ServiceRequests.Add(sr);
        }
        ServiceRequestsView = new ListCollectionView(ServiceRequests);
    }

    public Task WindowClosingAsync()
    {
        // TODO : Check outstanding edits have been saved
        return Task.CompletedTask;
    }
}
