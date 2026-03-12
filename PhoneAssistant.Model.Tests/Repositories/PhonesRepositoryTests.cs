namespace PhoneAssistant.Model.Tests;

public sealed class PhonesRepositoryTests : DbTestHelper
{
    readonly DbTestHelper _helper = new();
    readonly PhonesRepository _repository;

    const string ASSET_TAG = "asset";
    const string CONDITION_N = "N";
    const string CONDITION_R = "R";
    const string DESPATCH_DETAILS = "Despatch";
    const string FORMER_USER = "former user";
    const string IMEI = "imei";
    const string MODEL = "model";
    const string NEW_USER = "new user";
    const string NOTES = "notes";
    const string PHONE_NUMBER = "phone number";
    const string SIM_NUMBER = "sim number";
    const string STATUS = "Production";
    const int TICKET = 12345;

    readonly Phone _phone = new()
    {
        AssetTag = ASSET_TAG,
        Condition = CONDITION_R,
        DespatchDetails = DESPATCH_DETAILS,
        Esim = true,
        FormerUser = FORMER_USER,
        Imei = IMEI,
        IncludeOnTrackingSheet = true,
        Model = MODEL,
        NewUser = NEW_USER,
        Notes = NOTES,
        OEM = Manufacturer.Nokia,
        PhoneNumber = PHONE_NUMBER,
        SimNumber = SIM_NUMBER,
        Status = STATUS,
        Ticket = TICKET
    };

    public PhonesRepositoryTests()
    {
        _repository = new(_helper.DbContext);
    }

    [Test]
    public async Task AssetTagUnique_ShouldReturnTrue_WhenAssetTagNull()
    {
        bool actual = await _repository.AssetTagUniqueAsync(null);

        await Assert.That(actual).IsTrue();
    }

    [Test]
    public async Task AssetTagUnique_ShouldReturnTrue_WhenPhoneDoesNotExist()
    {
        bool actual = await _repository.AssetTagUniqueAsync("DoesNotExist");

        await Assert.That(actual).IsTrue();
    }

    [Test]
    public async Task AssetTagUnique_ShouldReturnFalse_WhenPhoneDoesExistAsync()
    {
        _helper.DbContext.Phones.Add(_phone);
        await _helper.DbContext.SaveChangesAsync();

        bool actual = await _repository.AssetTagUniqueAsync(_phone.AssetTag);

        await Assert.That(actual).IsFalse();
    }

    [Test]
    public async Task ConcurrentChange_should_return_false_when_no_concurrent_change()
    {
        _helper.DbContext.Phones.Add(_phone);
        await _helper.DbContext.SaveChangesAsync();

        bool concurrentChange = await _repository.ConcurrentChange(_phone.Imei, _phone.LastUpdate);
        await Assert.That(concurrentChange).IsFalse();
    }

    [Test]
    public async Task ConcurrentChange_should_return_true_when_concurrent_change()
    {
        await _repository.CreateAsync(_phone);
        string originalLastUpdate = _phone.LastUpdate;
        await Task.Delay(1100); // Ensure LastUpdate will be different on updated record
        _phone.Notes = "Changed note";
        await _repository.UpdateAsync(_phone);

        bool concurrentChange = await _repository.ConcurrentChange(_phone.Imei, originalLastUpdate);

        await Assert.That(concurrentChange).IsTrue();
    }

    [Test]
    public async Task Exists_ShouldReturnFalse_WhenPhoneDoesNotExist()
    {
        bool actual = await _repository.ExistsAsync("DoesNotExist");

        await Assert.That(actual).IsFalse();
    }

    [Test]
    public async Task Exists_ShouldReturnTrue_WhenPhoneDoesExistAsync()
    {
        _helper.DbContext.Phones.Add(_phone);
        await _helper.DbContext.SaveChangesAsync();

        bool actual = await _repository.ExistsAsync(_phone.Imei);

        await Assert.That(actual).IsTrue();
    }

    [Test]
    public async Task UpdateAsync_WithNullPhone_ThrowsException()
    {
#pragma warning disable CS8600, CS8604 // Converting null literal or possible null value to non-nullable type.
        Phone phone = null;
        await Assert.ThrowsAsync<NullReferenceException>(() => _repository.UpdateAsync(phone));
#pragma warning restore CS8600, CS8604 // Possible null reference argument.
    }

    [Test]
    [Arguments(nameof(Phone.AssetTag))]
    [Arguments(nameof(Phone.Condition))]
    [Arguments(nameof(Phone.DespatchDetails))]
    [Arguments(nameof(Phone.Esim))]
    [Arguments(nameof(Phone.FormerUser))]
    [Arguments(nameof(Phone.IncludeOnTrackingSheet))]
    [Arguments(nameof(Phone.Model))]
    [Arguments(nameof(Phone.NewUser))]
    [Arguments(nameof(Phone.Notes))]
    [Arguments(nameof(Phone.OEM))]
    [Arguments(nameof(Phone.PhoneNumber))]
    [Arguments(nameof(Phone.SimNumber))]
    [Arguments(nameof(Phone.Status))]
    [Arguments(nameof(Phone.Ticket))]
    public async Task UpdateAsync_with_found_Phone_and_changed_property_should_return_Updated(string propertyName)
    {
        Phone updated = new()
        {
            Imei = IMEI,
            AssetTag = ASSET_TAG,
            Condition = CONDITION_R,
            DespatchDetails = DESPATCH_DETAILS,
            Esim = true,
            FormerUser = FORMER_USER,
            IncludeOnTrackingSheet = true,
            Model = MODEL,
            NewUser = NEW_USER,
            Notes = NOTES,
            OEM = Manufacturer.Nokia,
            PhoneNumber = PHONE_NUMBER,
            SimNumber = SIM_NUMBER,
            Status = STATUS,
            Ticket = TICKET
        };
        switch (propertyName)
        {
            case nameof(Phone.AssetTag):
                updated.AssetTag = "U" + ASSET_TAG;
                break;
            case nameof(Phone.Condition):
                updated.Condition = CONDITION_N;
                break;
            case nameof(Phone.DespatchDetails):
                updated.DespatchDetails = "U" + DESPATCH_DETAILS;
                break;
            case nameof(Phone.Esim):
                updated.Esim = false;
                break;
            case nameof(Phone.FormerUser):
                updated.FormerUser = "U" + FORMER_USER;
                break;
            case nameof(Phone.IncludeOnTrackingSheet):
                updated.IncludeOnTrackingSheet = false;
                break;
            case nameof(Phone.Model):
                updated.Model = "U" + MODEL;
                break;
            case nameof(Phone.NewUser):
                updated.NewUser = "U" + NEW_USER;
                break;
            case nameof(Phone.Notes):
                updated.Notes = "U" + NOTES;
                break;
            case nameof(Phone.OEM):
                updated.OEM = Manufacturer.Other;
                break;
            case nameof(Phone.PhoneNumber):
                updated.PhoneNumber = "U" + PHONE_NUMBER;
                break;
            case nameof(Phone.SimNumber):
                updated.SimNumber = "U" + SIM_NUMBER;
                break;
            case nameof(Phone.Status):
                updated.Status = "U" + STATUS;
                break;
            case nameof(Phone.Ticket):
                updated.Ticket = TICKET + 1;
                break;
            default:
                Assert.Fail($"Unknown property: {propertyName}");
                break;
        }
        await _helper.DbContext.Phones.AddAsync(_phone);
        await _helper.DbContext.SaveChangesAsync();

        UpdateStatus result = await _repository.UpdateAsync(updated);

        await Assert.That(result).IsEqualTo(UpdateStatus.Updated);
        Phone? actual = await _helper.DbContext.Phones.FindAsync([_phone.Imei]);
        await Assert.That(actual).IsNotNull();

    }

    [Test]
    public async Task UpdateAsync_with_unchanged_properties_does_not_update_database()
    {
        await _helper.DbContext.Phones.AddAsync(_phone);
        await _helper.DbContext.SaveChangesAsync();
        Phone? original = await _helper.DbContext.Phones.FindAsync([_phone.Imei]);

        UpdateStatus result = await _repository.UpdateAsync(_phone);

        await Assert.That(original).IsNotNull();
        await Assert.That(result).IsEqualTo(UpdateStatus.Unchanged);
        Phone? actual = await _helper.DbContext.Phones.FindAsync([_phone.Imei]);
        await Assert.That(actual).IsNotNull();
        await Assert.That(actual.LastUpdate).IsEqualTo(original.LastUpdate);
    }

    [Test]
    public async Task UpdateAsync_WithPhoneNotFound_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.UpdateAsync(_phone));
    }


    [Test]
    public async Task UpdateStatusAsync_WithNullImei_ThrowsException()
    {
#pragma warning disable CS8604 // Converting null literal or possible null value to non-nullable type.
        string? imei = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateStatusAsync(imei, "ApplicationSettings.StatusDisposed"));
#pragma warning restore CS8604 // Possible null reference argument.
    }

    [Test]
    public async Task UpdateStatusAsync_WithNullStatus_ThrowsException()
    {
#pragma warning disable CS8604 // Converting null literal or possible null value to non-nullable type.
        string? status = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateStatusAsync("imei", status));
#pragma warning restore CS8604 // Possible null reference argument.
    }

    [Test]
    public async Task UpdateStatusAsync_WithPhoneNotFound_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.UpdateStatusAsync("not found", ApplicationConstants.StatusDisposed));
    }

    [Test]
    public async Task UpdateStatusAsync_WithStatusChange_Succeeds()
    {
        _phone.Status = ApplicationConstants.StatusDecommissioned;
        await _helper.DbContext.Phones.AddAsync(_phone);
        await _helper.DbContext.SaveChangesAsync();

        await _repository.UpdateStatusAsync(_phone.Imei, ApplicationConstants.StatusDisposed);

        Phone? actual = await _helper.DbContext.Phones.FindAsync(_phone.Imei);
        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.Status).IsEqualTo(ApplicationConstants.StatusDisposed);
    }

    [Test]
    public async Task UserHasProductionPhone_should_return_false_when_no_Production_phone_found()
    {
        Phone[] phones = [
            new Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "N", OEM = Manufacturer.Nokia, Status = "In Repair", NewUser = "new user"},
            new Phone() { Imei = "2" , AssetTag = "Tag B2", Model = "", Condition = "N", OEM = Manufacturer.Samsung, Status = "Decommissioned", NewUser = "new user"},
            new Phone() { Imei = "3" , AssetTag = "Tag C3", Model = "", Condition = "N", OEM = Manufacturer.Nokia, Status = "Disposed", NewUser = "new user"},
            new Phone() { Imei = "4" , AssetTag = "Tag D4", Model = "", Condition = "N", OEM = Manufacturer.Other, Status = "Decommissioned", NewUser = "new user"},
            new Phone() { Imei = "5" , AssetTag = "Tag E5", Model = "", Condition = "N", OEM = Manufacturer.Apple, Status = "Disposed", NewUser = "new user"}
            ];
        _helper.DbContext.Phones.AddRange(phones);
        _helper.DbContext.SaveChanges();

        bool actual = await _repository.UserHasProductionPhone("new user");

        await Assert.That(actual).IsFalse();
    }

    [Test]
    public async Task UserHasProductionPhone_should_return_true_when_Production_phone_found()
    {
        Phone[] phones = [
            new Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "N", OEM = Manufacturer.Nokia, Status = "In Repair", NewUser = "new user"},
            new Phone() { Imei = "2" , AssetTag = "Tag B2", Model = "", Condition = "N", OEM = Manufacturer.Samsung, Status = "Production", NewUser = "new user"},
            new Phone() { Imei = "3" , AssetTag = "Tag C3", Model = "", Condition = "N", OEM = Manufacturer.Nokia, Status = "Production", NewUser = "new user"},
            new Phone() { Imei = "4" , AssetTag = "Tag D4", Model = "", Condition = "N", OEM = Manufacturer.Other, Status = "Decommissioned", NewUser = "new user"},
            new Phone() { Imei = "5" , AssetTag = "Tag E5", Model = "", Condition = "N", OEM = Manufacturer.Apple, Status = "Disposed", NewUser = "new user"}
            ];
        _helper.DbContext.Phones.AddRange(phones);
        _helper.DbContext.SaveChanges();

        bool actual = await _repository.UserHasProductionPhone("new user");

        await Assert.That(actual).IsTrue();
    }

    [Test]
    [Arguments("new user")]
    [Arguments("New user")]
    [Arguments("New User")]
    [Arguments("NEW USER")]
    public async Task UserHasProductionPhone_is_case_insensitive(string newUser)
    {
        Phone phone = new() { Imei = "1", AssetTag = "Tag A1", Model = "", Condition = "N", OEM = Manufacturer.Samsung, Status = "Production", NewUser = "NEW USER" };
        await _helper.DbContext.Phones.AddAsync(phone);
        _helper.DbContext.SaveChanges();

        bool actual = await _repository.UserHasProductionPhone(newUser);

        await Assert.That(actual).IsTrue();
    }
}
