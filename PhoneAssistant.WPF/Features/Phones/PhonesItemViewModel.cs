﻿using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesItemViewModel : ObservableObject
{
    private readonly IPhonesRepository _repository;
    private readonly IPrintEnvelope _printEnvelope;
    private readonly v1Phone _phone;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public PhonesItemViewModel(IPhonesRepository repository, IPrintEnvelope printEnvelope, v1Phone phone)
    {
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
        _phone = phone ?? throw new ArgumentNullException(nameof(phone));

        AssetTag = phone.AssetTag ?? string.Empty;
        FormerUser = phone.FormerUser ?? string.Empty;
        Imei = phone.Imei;
        LastUpdate = phone.LastUpdate ?? string.Empty;
        Model = phone.Model ?? string.Empty;
        NewUser = phone.NewUser ?? string.Empty;
        NorR = phone.NorR ?? string.Empty;
        Notes = phone.Notes ?? string.Empty;
        OEM = phone.OEM ?? string.Empty;
        PhoneNumber = phone.PhoneNumber ?? string.Empty;
        SimNumber = phone.SimNumber ?? string.Empty;
        SR = phone.SR.ToString() ?? string.Empty;
        Status = phone.Status ?? string.Empty;

        if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(SimNumber))
            CanRemoveSim = false;
        else
            CanRemoveSim = true;
        if (_phone.Status == "Production")
            CanPrintEnvelope = true;
        else
            CanPrintEnvelope = false;
    }

    [ObservableProperty]
    private string _assetTag;

    [ObservableProperty]
    private string _formerUser;

    [ObservableProperty]
    private string _imei;

    [ObservableProperty]   
    private string _lastUpdate;

    [ObservableProperty]
    private string _model;

    [ObservableProperty]
    private string _newUser;

    [ObservableProperty]
    private string _norR;

    [ObservableProperty]
    private string _notes;

    [ObservableProperty]
    private string _oEM;

    [ObservableProperty]
    private string _phoneNumber;

    [ObservableProperty]
    private string _simNumber;

    [ObservableProperty]
    private string _sR;

    [ObservableProperty]
    private string _status;


    [RelayCommand]
    private void PrintEnvelope()
    {
        _printEnvelope.Execute(_phone);
        CanPrintEnvelope = false;
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private bool canPrintEnvelope;

    [RelayCommand]
    private void RemoveSim()
    {
        _repository.RemoveSimFromPhone(_phone);
        PhoneNumber = string.Empty;
        SimNumber = string.Empty;
        CanRemoveSim = false;
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private bool canRemoveSim;
}
