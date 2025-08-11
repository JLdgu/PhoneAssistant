using Microsoft.EntityFrameworkCore;

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
    const int SR = 12345;
    const string STATUS = "Production";

    readonly Phone _phone = new()
    {
        AssetTag = ASSET_TAG,
        Condition = CONDITION_R,
        DespatchDetails = DESPATCH_DETAILS,
        FormerUser = FORMER_USER,
        Imei = IMEI,
        Model = MODEL,
        NewUser = NEW_USER,
        Notes = NOTES,
        OEM = Manufacturer.Nokia,
        PhoneNumber = PHONE_NUMBER,
        SimNumber = SIM_NUMBER,
        SR = SR,
        Status = STATUS
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
    public async Task UpdateAsync_WithNullPhone_ThrowsException()
    {
#pragma warning disable CS8600, CS8604 // Converting null literal or possible null value to non-nullable type.
        Phone phone = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateAsync(phone));
#pragma warning restore CS8600, CS8604 // Possible null reference argument.
    }

    [Test]
    public async Task UpdateAsync_WithPhoneNotFound_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.UpdateAsync(_phone));
    }

    [Test]
    public async Task UpdateAsync_WithPhoneFound_Succeeds()
    {
        Phone original = new()
        {
            Imei = _phone.Imei,
            Condition = CONDITION_N,
            Model = "old model",
            OEM = Manufacturer.Apple,
            Status = ApplicationConstants.Statuses[1]
        };
        await _helper.DbContext.Phones.AddAsync(original);
        await _helper.DbContext.SaveChangesAsync();
        string lastUpdate = original.LastUpdate;

        await _repository.UpdateAsync(_phone);

        Phone? actual = await _helper.DbContext.Phones.FindAsync(new object?[] { _phone.Imei });
        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.AssetTag).IsEqualTo(_phone.AssetTag);
        await Assert.That(actual.Condition).IsEqualTo(_phone.Condition);
        await Assert.That(actual.DespatchDetails).IsEqualTo(_phone.DespatchDetails);
        await Assert.That(actual.FormerUser).IsEqualTo(_phone.FormerUser);
        await Assert.That(actual.Imei).IsEqualTo(_phone.Imei);
        await Assert.That(actual.Model).IsEqualTo(_phone.Model);
        await Assert.That(actual.NewUser).IsEqualTo(_phone.NewUser);
        await Assert.That(actual.Notes).IsEqualTo(_phone.Notes);
        await Assert.That(actual.OEM).IsEqualTo(_phone.OEM);
        await Assert.That(actual.PhoneNumber).IsEqualTo(_phone.PhoneNumber);
        await Assert.That(actual.SimNumber).IsEqualTo(_phone.SimNumber);
        await Assert.That(actual.SR).IsEqualTo(_phone.SR);
        await Assert.That(actual.Status).IsEqualTo(_phone.Status);

        UpdateHistoryPhone? history = await _helper.DbContext.UpdateHistoryPhones.FirstOrDefaultAsync(h => h.Id > 0);
        await Assert.That(history).IsNotNull();
        await Assert.That(actual.AssetTag).IsEqualTo(history!.AssetTag);
        await Assert.That(actual.Condition).IsEqualTo(history.Condition);
        await Assert.That(actual.DespatchDetails).IsEqualTo(history.DespatchDetails);
        await Assert.That(actual.FormerUser).IsEqualTo(history.FormerUser);
        await Assert.That(actual.Imei).IsEqualTo(history.Imei);
        await Assert.That(actual.Model).IsEqualTo(history.Model);
        await Assert.That(actual.NewUser).IsEqualTo(history.NewUser);
        await Assert.That(actual.Notes).IsEqualTo(history.Notes);
        await Assert.That(actual.OEM).IsEqualTo(history.OEM);
        await Assert.That(actual.PhoneNumber).IsEqualTo(history.PhoneNumber);
        await Assert.That(actual.SimNumber).IsEqualTo(history.SimNumber);
        await Assert.That(actual.SR).IsEqualTo(history.SR);
        await Assert.That(actual.Status).IsEqualTo(history.Status);
    }

    [Test]
    public async Task UpdateAsync_WithDuplicateUpdate()
    {
        await _helper.DbContext.Phones.AddAsync(
            new Phone()
            {
                Imei = _phone.Imei,
                Condition = CONDITION_N,
                Model = "old model",
                OEM = Manufacturer.Apple,
                Status = ApplicationConstants.Statuses[1]
            });
        await _helper.DbContext.SaveChangesAsync();

        await _repository.UpdateAsync(_phone);

        await _repository.UpdateAsync(_phone);
        _ = await _helper.DbContext.Phones.FindAsync(_phone.Imei);

        UpdateHistoryPhone? history = await _helper.DbContext.UpdateHistoryPhones.FindAsync(1);
        await Assert.That(history).IsNotNull();

        history = await _helper.DbContext.UpdateHistoryPhones.FindAsync(2);
        await Assert.That(history).IsNull();
    }    
}
