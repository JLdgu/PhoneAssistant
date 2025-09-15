using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesItemViewModel : ObservableObject
{
    private readonly IApplicationSettingsRepository _appSettings;
    private readonly IBaseReportRepository _baseReportRepository;
    private readonly IPhonesRepository _repository;
    private readonly IMessenger _messenger;
    private readonly Phone _phone;

    public PhonesItemViewModel(IApplicationSettingsRepository appSettings,
                               IBaseReportRepository baseReportRepository,
                               IPhonesRepository repository,
                               IMessenger messenger,
                               Phone phone)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _baseReportRepository = baseReportRepository ?? throw new ArgumentNullException(nameof(baseReportRepository));
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
        SerialNumber = phone.SerialNumber ?? string.Empty;
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

        await UpdatePhone();
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

        await UpdatePhone();
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
        await UpdatePhone();
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

        await UpdatePhone();
    }

    [ObservableProperty]
    private string _norR;
    async partial void OnNorRChanged(string value)
    {
        if (value == _phone.Condition) return;

        _phone.Condition = value;

        await UpdatePhone();
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

        await UpdatePhone();
    }

    [ObservableProperty]
    private Manufacturer _oEM;
    async partial void OnOEMChanged(Manufacturer value)
    {
        if (value == _phone.OEM) return;

        _phone.OEM = value;
        await UpdatePhone();        
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
            string? simNumber = await _baseReportRepository.GetSimNumberAsync(value);
            if (simNumber is not null)
            {
                _phone.SimNumber = simNumber;
                SimNumber = simNumber;
            }
        }

        await UpdatePhone();
    }

    [ObservableProperty]
    private string _serialNumber;
    async partial void OnSerialNumberChanged(string value)
    {
        if (value == _phone.SerialNumber) return;
        if (string.IsNullOrEmpty(value) && _phone.SerialNumber is null) return;
        if (string.IsNullOrEmpty(value))
            _phone.SerialNumber = null;
        else
            _phone.SerialNumber = value;
        await UpdatePhone();
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

        await UpdatePhone();
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

        await UpdatePhone();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateEmailCommand))]
    private string _status;
    async partial void OnStatusChanged(string value)
    {
        if (value == _phone.Status) return;

        _multiUpdate = true;
        if (value == "In Stock" || value == "In Repair" || value == "Decommissioned")
        {
            _phone.DespatchDetails = null;
            FormerUser = NewUser;
            NewUser = string.Empty;
            SR = string.Empty;
        }
        if (value == "Decommissioned")
        {
            SR = _appSettings.ApplicationSettings.DefaultDecommissionedTicket.ToString();             
        }

        if (value == "In Stock" || value == "Decommissioned") 
        {
            Notes = string.Empty;
        }
        _multiUpdate = false;

        _phone.Status = value;

        await UpdatePhone();
    }

    private bool _multiUpdate = false;
    private async Task UpdatePhone()
    {
        if (_multiUpdate) return;

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
        if (!_phone.Status.Equals(ApplicationConstants.StatusProduction, StringComparison.InvariantCultureIgnoreCase))
            return false;
        if (string.IsNullOrEmpty(SR))
            return false;
        if (string.IsNullOrEmpty(NewUser))
            return false;

        return true;
    }

    [RelayCommand(CanExecute = nameof(CanRemoveSim))]
    private void RemoveSim()
    {
        PhoneNumber = string.Empty;
        SimNumber = string.Empty;
        LastUpdate = _phone.LastUpdate;
    }
    private bool CanRemoveSim() => !(string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(SimNumber));
}
