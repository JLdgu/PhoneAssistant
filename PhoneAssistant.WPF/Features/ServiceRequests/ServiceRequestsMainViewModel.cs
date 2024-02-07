using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.ServiceRequests;

public sealed partial class ServiceRequestsMainViewModel : ObservableObject, IServiceRequestsMainViewModel
{
    private readonly IServiceRequestsRepository _serviceRequestsRepository;

    public ServiceRequestsMainViewModel(IServiceRequestsRepository serviceRequestsRepository)
    {
        _serviceRequestsRepository = serviceRequestsRepository;

        ServiceRequestsItemViewModel = new ServiceRequestsItemViewModel(serviceRequestsRepository);
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
        await Task.CompletedTask;
    }
}
