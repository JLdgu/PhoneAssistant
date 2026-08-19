using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

using System.Collections.ObjectModel;
using System.Windows;

namespace PhoneAssistant.WPF.Features.BaseReport;

public interface IBaseReportMainViewModel : IViewModel
{
}

public partial class BaseReportMainViewModel(ISimRepository repository) : ViewModelBase, IBaseReportMainViewModel
{
    private readonly ISimRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ObservableCollection<BaseReportSim> BaseReportSims { get; } = [];

    [ObservableProperty]
    public partial bool? Esim { get; set; }

    [ObservableProperty]
    public partial string LatestImport { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool NoResultsFound { get; set; } = false;

    [ObservableProperty]
    public partial string SearchPhoneNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchSimNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchUserName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Visibility ProgressVisibility { get; set; } = Visibility.Collapsed;

    [RelayCommand]
    private async Task PhoneNumberSearch()
    {
        if (string.IsNullOrEmpty(SearchPhoneNumber) && string.IsNullOrEmpty(SearchSimNumber)) return;

        IEnumerable<Sim> sims = await _repository.GetSimsForPhoneNumber(SearchPhoneNumber);
        await LoadSimsIntoCollection(sims);
    }

    [RelayCommand]
    private async Task SimNumberSearch()
    {
        if (string.IsNullOrEmpty(SearchSimNumber)) return;

        IEnumerable<Sim> sims = await _repository.GetSimsForSimNumber(SearchSimNumber);
        await LoadSimsIntoCollection(sims);
    }

    [RelayCommand]
    private async Task UserNameSearch()
    {
        if (string.IsNullOrEmpty(SearchUserName)) return;

        IEnumerable<Sim> sims = await _repository.GetSimsForUserName(SearchUserName);
        await LoadSimsIntoCollection(sims);
    }

    private async Task LoadSimsIntoCollection(IEnumerable<Sim> sims)
    {
        BaseReportSims.Clear();
        ProgressVisibility = Visibility.Visible;
        await Task.Delay(500); // Allow UI to update before starting the search

        try
        {
            if (!sims.Any())
            {
                NoResultsFound = true;
                return;
            }
            ulong maxBoadbandData = sims.Max(s => s.BroadbandData);
            foreach (var sim in sims)
            {
                BaseReportSim baseReportSim = new(sim, maxBoadbandData);
                BaseReportSims.Add(baseReportSim);
            }
        }
        finally
        {
            ProgressVisibility = Visibility.Collapsed;
        }

        return;
    }

    public override async Task LoadAsync()
    {
        LatestImport = $"Latest Import: {await _repository.GetLatestBillingPeriod()}";
    }
}

public sealed class BaseReportSim : Sim
{
    private const int MaxBarWidth = 200;
    public string BroadbandDataText { get; init; }
    public int BarWidth { get; init; }
    public int FillWidth { get; init; }

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute]
    public BaseReportSim(Sim sim, ulong maxBroadbandData)
    {
        PhoneNumber = sim.PhoneNumber;
        SIMNumber = sim.SIMNumber;
        BillingPeriod = sim.BillingPeriod;
        UserName = sim.UserName;
        BroadbandData = sim.BroadbandData;
        TextMessages = sim.TextMessages;
        VoiceCalls = sim.VoiceCalls;
        Esim = sim.Esim;

        BroadbandDataText = FormatBytes(sim.BroadbandData);
        if (BroadbandData == 0)
        {
            BarWidth = 0;
            FillWidth = MaxBarWidth;
            return;
        }
        if (BroadbandData == maxBroadbandData)
        {
            BarWidth = MaxBarWidth;
            FillWidth = 0;
            return;
        }
        BarWidth = (int)(MaxBarWidth * sim.BroadbandData / maxBroadbandData);
        FillWidth = MaxBarWidth - BarWidth;
    }

    private static string FormatBytes(ulong bytes)
    {
        string[] suffixes = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];

        if (bytes == 0)
            return "0 B";

        int order = 0;
        double remainder = bytes;

        while (remainder >= 1000 && order < suffixes.Length - 1)
        {
            order++;
            remainder /= 1000;
        }

        return $"{remainder:0.##} {suffixes[order]}";
    }
}
