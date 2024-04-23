using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesItemViewModel : ObservableObject
{
    private readonly IPhonesRepository _repository;
    private readonly ISimsRepository _simsRepository;
    private readonly IPrintEnvelope _printEnvelope;
    private readonly IMessenger _messenger;
    private readonly Phone _phone;

    public PhonesItemViewModel(IPhonesRepository repository,
                               ISimsRepository simsRepository,
                               IPrintEnvelope printEnvelope,
                               IMessenger messenger,
                               Phone phone)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _simsRepository = simsRepository ?? throw new ArgumentNullException(nameof(simsRepository));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _phone = phone ?? throw new ArgumentNullException(nameof(phone));

        AssetTag = phone.AssetTag ?? string.Empty;
        FormerUser = phone.FormerUser ?? string.Empty;
        Imei = phone.Imei;
        LastUpdate = phone.LastUpdate ?? string.Empty;
        Model = phone.Model ?? string.Empty;
        NewUser = phone.NewUser ?? string.Empty;
        NorR = phone.Condition ?? string.Empty;
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
        if (string.IsNullOrEmpty(value))
            _phone.AssetTag = null;
        else
            _phone.AssetTag = value;

        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
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

        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
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
        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
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

        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
    }

    [ObservableProperty]
    private string _norR;
    async partial void OnNorRChanged(string value)
    {
        if (value == _phone.Condition) return;

        _phone.Condition = value;
        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
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

        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
    }

    [ObservableProperty]
    private OEMs _oEM;
    async partial void OnOEMChanged(OEMs value)
    {
        if (value == _phone.OEM) return;

        _phone.OEM = value;
        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SimNumber))]
    [NotifyCanExecuteChangedFor(nameof(RemoveSimCommand))]
    private string _phoneNumber;
    async partial void OnPhoneNumberChanged(string value)
    {
        if (value == _phone.PhoneNumber) return;

        if (string.IsNullOrEmpty(value) && _phone.PhoneNumber is null) return;
        if (string.IsNullOrEmpty(value))
             _phone.PhoneNumber = null;
        else
        {
            _phone.PhoneNumber = value;
            string? simNumber = await _simsRepository.DeleteSIMAsync(value);
            if (simNumber is not null)
            {
                _phone.SimNumber = simNumber;
                SimNumber = simNumber;
            }

        }

        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveSimCommand))]
    private string _simNumber;
    async partial void OnSimNumberChanged(string value)
    {
        if (value == _phone.SimNumber) return;

        if (string.IsNullOrEmpty(value) && _phone.SimNumber is null) return;
        if (string.IsNullOrEmpty(value))           
            _phone.SimNumber = null;
        else
            _phone.SimNumber = value;

        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
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

        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateEmailCommand))]
    private string _status;
    async partial void OnStatusChanged(string value)
    {
        if (value == _phone.Status) return;

        _phone.Status = value;
        await _repository.UpdateAsync(_phone);
        LastUpdate = _phone.LastUpdate;
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
        await _repository.RemoveSimFromPhoneAsync(_phone);
        PhoneNumber = string.Empty;
        SimNumber = string.Empty;
        LastUpdate = _phone.LastUpdate;
    }
    private bool CanRemoveSim() => !(string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(SimNumber));
}
