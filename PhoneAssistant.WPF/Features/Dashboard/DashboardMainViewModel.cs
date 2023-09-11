using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Phones;

namespace PhoneAssistant.WPF.Features.Dashboard;
public partial class DashboardMainViewModel : ObservableObject, IDashboardMainViewModel
{
    private readonly v1PhoneAssistantDbContext _dbContext;
    private readonly IPrintEnvelope _printEnvelope;

    public ObservableCollection<v1Phone> Phones { get; } = new();

    public DashboardMainViewModel(v1PhoneAssistantDbContext dbContext, IPrintEnvelope printEnvelope)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
    }
    
    [ObservableProperty]
    private v1Phone? _selectedPhone;

    partial void OnSelectedPhoneChanged(v1Phone? value)
    {
        if (SelectedPhone is null)
        {
            CanPrintEnvelope = false;
        }
        else
        {
            CanPrintEnvelope = true;
        }
    }

    [RelayCommand]
    private void PrintEnvelope()
    {
        if (SelectedPhone is null) return;

        CanPrintEnvelope = false;
        _printEnvelope.Execute(SelectedPhone);
    }

    [ObservableProperty]
    private bool canPrintEnvelope;

    public async Task LoadAsync()
    {
        Phones.Clear();
        await foreach (var phone in _dbContext.Phones.AsAsyncEnumerable())
        {
            Phones.Add(phone);
        }        
    }

    public Task WindowClosingAsync()
    {
        throw new NotImplementedException();
    }

}