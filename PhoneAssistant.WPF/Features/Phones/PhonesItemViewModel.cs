﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesItemViewModel : ObservableObject
{
    private readonly IPhonesRepository _repository;
    private readonly IPrintEnvelope _printEnvelope;
    private readonly IMessenger _messenger;
    private Phone _phone;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public PhonesItemViewModel(IPhonesRepository repository,
                               IPrintEnvelope printEnvelope,
                               IMessenger messenger,
                               Phone phone)
    {
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _phone = phone ?? throw new ArgumentNullException(nameof(phone));

        AssetTag = phone.AssetTag ?? string.Empty;
        FormerUser = phone.FormerUser ?? string.Empty;
        Imei = phone.Imei;
        LastUpdate = phone.LastUpdate ?? string.Empty;
        Model = phone.Model ?? string.Empty;
        NewUser = phone.NewUser ?? string.Empty;
        NorR = phone.NorR ?? string.Empty;
        Notes = phone.Notes ?? string.Empty;
        OEM = phone.OEM;
        PhoneNumber = phone.PhoneNumber ?? string.Empty;
        SimNumber = phone.SimNumber ?? string.Empty;
        SR = phone.SR.ToString() ?? string.Empty;
        Status = phone.Status ?? string.Empty;
    }

    #region ObServableProperties
    [ObservableProperty]
    private string _assetTag;
    async partial void OnAssetTagChanged(string value)
    {
        if (value == _phone.AssetTag) return;

        if (string.IsNullOrEmpty(value) && _phone.AssetTag is null) return;

        _phone.AssetTag = value;
        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }

    [ObservableProperty]
    private string _formerUser;
    async partial void OnFormerUserChanged(string value)
    {
        if (value == _phone.FormerUser) return;

        if (string.IsNullOrEmpty(value))
        {
            if (_phone.FormerUser is null)
            {
                return;
            }
            else
            {
                _phone.FormerUser = null;
            }
        }
        else
        {
            _phone.FormerUser = value;
        }

        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }

    [ObservableProperty]
    private string _imei;

    [ObservableProperty]
    private string _lastUpdate;

    [ObservableProperty]
    private string _model;
    async partial void OnModelChanged(string value)
    {
        if (value == _phone.Model) return;

        if (string.IsNullOrEmpty(value) && _phone.AssetTag is null) return;

        _phone.Model = value;
        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateEmailCommand))]
    private string _newUser;
    async partial void OnNewUserChanged(string value)
    {
        if (value == _phone.NewUser) return;

        if (string.IsNullOrEmpty(value))
        {
            if (_phone.NewUser is null)
            {
                return;
            }
            else
            {
                _phone.NewUser = null;
            }
        }
        else
        {
            _phone.NewUser = value;
        }

        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }

    [ObservableProperty]
    private string _norR;
    async partial void OnNorRChanged(string value)
    {
        if (value == _phone.NorR) return;

        _phone.NorR = value;
        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }

    [ObservableProperty]
    private string _notes;
    async partial void OnNotesChanged(string value)
    {
        if (value == _phone.Notes) return;

        if (string.IsNullOrEmpty(value))
        {
            if (_phone.Notes is null)
            {
                return;
            }
            else
            {
                _phone.Notes = null;
            }
        }
        else
        {
            _phone.Notes = value;
        }

        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }

    [ObservableProperty]
    private OEMs _oEM;
    async partial void OnOEMChanged(OEMs value)
    {
        if (value == _phone.OEM) return;

        _phone.OEM = value;
        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveSimCommand))]
    private string _phoneNumber;
    async partial void OnPhoneNumberChanged(string value)
    {
        if (value == _phone.PhoneNumber) return;
        if (string.IsNullOrEmpty(value))
        {
            if (_phone.PhoneNumber is null)
            {
                return;
            }
            else
            {
                _phone.PhoneNumber = null;
            }
        }
        else
        {
            _phone.PhoneNumber = value;
        }

        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveSimCommand))]
    private string _simNumber;
    async partial void OnSimNumberChanged(string value)
    {
        if (value == _phone.SimNumber) return;
        if (string.IsNullOrEmpty(value))
        {
            if (_phone.SimNumber is null)
            {
                return;
            }
            else
            {
                _phone.SimNumber = null;
            }
        }
        else
        {
            _phone.SimNumber = value;
        }

        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateEmailCommand))]
    private string _sR;
    async partial void OnSRChanged(string value)
    {
        if (value == _phone.SR.ToString()) return;

        if (string.IsNullOrEmpty(value))
        {
            if (_phone.SR is null)
            {
                return;
            }
            else
            {
                _phone.SR = null;
            }
        }
        else
        {
            _phone.SR = int.Parse(value);
        }

        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateEmailCommand))]
    private string _status;
    async partial void OnStatusChanged(string value)
    {
        if (value == _phone.Status) return;

        _phone.Status = value;
        var lastUpdate = await _repository.UpdateAsync(_phone);
        LastUpdate = lastUpdate;
    }
    #endregion

    [RelayCommand(CanExecute = nameof(CanCreateEmail))]
    private void CreateEmail()
    {

        OrderDetails orderDetails = new(_phone);
        _messenger.Send(new Order(orderDetails));
    }
    private bool CanCreateEmail()
    {
        if (!_phone.Status.Equals("Production", StringComparison.InvariantCultureIgnoreCase))
            return false;
        if (string.IsNullOrEmpty(SR))
            return false;
        if (string.IsNullOrEmpty(NewUser))
            return false;

        return true;
    }

    [RelayCommand(CanExecute = nameof(CanRemoveSim))]
    private async Task RemoveSimAsync()
    {
        _phone = await _repository.RemoveSimFromPhone(_phone);
        PhoneNumber = string.Empty;
        SimNumber = string.Empty;
        LastUpdate = _phone.LastUpdate;
    }
    private bool CanRemoveSim() => !(string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(SimNumber));
}
