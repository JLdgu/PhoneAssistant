using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;
using System.ComponentModel.DataAnnotations;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed partial class SimsMainViewModel : ObservableValidator, ISimsMainViewModel
{
    private readonly IBaseReportRepository _baseReportRepository;
    private readonly IPrintEnvelope _printEnvelope;

    private readonly Phone _phone = new()
    {
        PhoneNumber = "07814209742",
        SimNumber = "8944122605563572205",
        Status = "status",
        AssetTag = "at",
        DespatchDetails = "dd",
        FormerUser = "fu",
        Imei = "imei",
        Model = "SIM Card",
        NewUser = "Rosie Lane",
        Condition = "norr",
        Notes = "note",
        OEM = Manufacturer.Apple,
        SR = 262281
    };

    public SimsMainViewModel(IBaseReportRepository baseReportRepository,
                             IPrintEnvelope printEnvelope)
    {
        _baseReportRepository = baseReportRepository ?? throw new ArgumentNullException(nameof(baseReportRepository));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));

        ValidateAllProperties();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    [NotifyDataErrorInfo]
    [Required]
    private string? _newUser;

    [RelayCommand(CanExecute = nameof(CanPrintEnvelope))]
    private async Task PrintEnvelope()
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(Ticket);
        int? ticket = int.Parse(Ticket);
        Phone phone = new()
        {
            PhoneNumber = "07814209742",
            SimNumber = "8944122605563572205",
            Status = "status",
            Imei = "imei",
            Model = "SIM Card",
            NewUser = NewUser,
            Condition = "norr",
            OEM = Manufacturer.Apple,
            SR = ticket
        };

        OrderDetails orderDetails = new(phone);

        await Task.Run(() => _printEnvelope.Execute(orderDetails));
    }
    private bool CanPrintEnvelope() => HasErrors == false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    [NotifyDataErrorInfo]
    [CustomValidation(typeof(Validation), nameof(Validation.ValidateTicket))]
    private string? _ticket;

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    

}
