using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed partial class SimsMainViewModel(IBaseReportRepository baseReportRepository,
                         IPrintEnvelope printEnvelope,
                         IServiceProvider serviceProvider) : ValidatableViewModel<SimsMainViewModel>(serviceProvider), ISimsMainViewModel
{
    private readonly IBaseReportRepository _baseReportRepository = baseReportRepository ?? throw new ArgumentNullException(nameof(baseReportRepository));
    private readonly IPrintEnvelope _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    private string? _newUser;
    async partial void OnNewUserChanged(string? value) => await ValidatePropertyAsync(nameof(NewUser));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    private string? _phoneNumber;
    async partial void OnPhoneNumberChanged(string? value)
    {
        await ValidatePropertyAsync(nameof(PhoneNumber));        

        SimNumber = await _baseReportRepository.GetSimNumberAsync(PhoneNumber!);
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    private string? _simNumber;
    async partial void OnSimNumberChanged(string? value) => await ValidatePropertyAsync(nameof(SimNumber));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    private string? _ticket;
    async partial void OnTicketChanged(string? value) => await ValidatePropertyAsync(nameof(Ticket));

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
            Ticket = ticket
        };

        OrderDetails orderDetails = new(phone);

        await Task.Run(() => _printEnvelope.Execute(orderDetails));
    }
    private bool CanPrintEnvelope() => HasErrors == false;
}
