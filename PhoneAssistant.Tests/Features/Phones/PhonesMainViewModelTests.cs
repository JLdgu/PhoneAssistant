using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.Messaging;

using Moq;
using Moq.AutoMock;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.Phones;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class PhonesMainViewModelTests
{
    [Test]
    public async Task ChangingFilterAssetTag_ChangesFilterViewAsync()
    {
        List<Phone> phones = [
            new Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() { Imei = "2" , AssetTag = "Tag Bb2", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "3", AssetTag = "Tag Ccc3", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""}
        ];
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterAssetTag = "B2";

        PhonesItemViewModel[] actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].AssetTag).IsEqualTo(phones[1].AssetTag );
    }

    [Test]
    [Arguments( null, 3)]
    [Arguments( false, 2)]
    [Arguments( true, 1)]
    public async Task ChangingFilterEsim_ChangesFilterView(bool? filterValue, int matchingCount)
    {
        List<Phone> phones = [
            new() { Esim = false, Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new() { Esim = true, Imei = "2" , AssetTag = "Tag Bb2", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new() { Esim = null, Imei = "3", AssetTag = "Tag Ccc3", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""}
        ];
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterEsim = filterValue;

        PhonesItemViewModel[] actual = [.. view.OfType<PhonesItemViewModel>()];
        await Assert.That(actual).Count().IsEqualTo(matchingCount);
    }

    [Test]
    public async Task ChangingFilterFormerUser_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1" , FormerUser = "Aa", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "2", FormerUser = "Bbb", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "3", FormerUser = "Ccc", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterFormerUser = "BB";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].FormerUser).IsEqualTo(phones[1].FormerUser);
    }

    [Test]
    public async Task ChangingFilterImei_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "11", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "22", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "33", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterImei = "22";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].Imei).IsEqualTo(phones[1].Imei);
    }

    [Test]
    public async Task ChangingFilterNorR_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1", Condition="N", Model = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() { Imei = "2", Condition="R", Model = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "3", Condition = "N", Model = "", OEM = Manufacturer.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterNorR = "R";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].NorR).IsEqualTo(phones[1].Condition);
    }

    [Test]
    public async Task ChangingFilterNewUser_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1" , NewUser = "User Aa", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "2", NewUser = "User Bbb", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "3", NewUser = "User Ccc", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterNewUser = "BB";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].NewUser).IsEqualTo(phones[1].NewUser);
    }

    [Test]
    public async Task ChangingFilterNotes_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1" , Notes = "Note1", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() { Imei = "2" , Notes = "Note2", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "3", Notes = "Note3", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterNotes = "e2";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].Notes).IsEqualTo(phones[1].Notes);
    }

    [Test]
    public async Task ChangingFilterOEM_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1" , OEM = Manufacturer.Apple, Model = "", Condition = "", Status = ""},
            new Phone() {Imei = "2", OEM = Manufacturer.Nokia, Model = "", Condition = "", Status = ""},
            new Phone() {Imei = "3", OEM = Manufacturer.Samsung, Model = "", Condition = "", Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterOEM = Manufacturer.Samsung;

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].OEM).IsEqualTo(phones[2].OEM);
    }

    [Test]
    public async Task ChangingFilterPhoneNumber_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() {Imei = "1", PhoneNumber = "01", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() { Imei = "2", PhoneNumber="02", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "3", PhoneNumber = "03", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterPhoneNumber = "02";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].PhoneNumber).IsEqualTo(phones[1].PhoneNumber);
    }

    [Test]
    public async Task ChangingFilterSerialNumber_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new() {Imei = "1", SimNumber = "101", Model = "", Condition = "", OEM = Manufacturer.Apple, SerialNumber = "aaa", Status = ""},
            new() { Imei = "2", SimNumber="202", Model = "", Condition = "", OEM = Manufacturer.Apple, SerialNumber = "bbb",Status = ""},
            new() {Imei = "3", SimNumber = "303", Model = "", Condition = "", OEM = Manufacturer.Apple, SerialNumber = "ccc",Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterSerialNumber = "b";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].SerialNumber).IsEqualTo(phones[1].SerialNumber);
    }

    [Test]
    public async Task ChangingFilterSimNumber_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() {Imei = "1", SimNumber = "101", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() { Imei = "2", SimNumber="202", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new Phone() {Imei = "3", SimNumber = "303", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterSimNumber = "02";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].SimNumber).IsEqualTo(phones[1].SimNumber);
    }

    [Test]
    public async Task ChangingFilterSR_ChangesFilterViewAsync()
    {
        List<Phone> phones = [
            new() { Imei = "1", Ticket=111, Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new() { Imei = "2", Ticket=222, Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new() {Imei = "3", Ticket = 333, Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""},
            new() {Imei = "4", Ticket = 112233, Model = "", Condition = "", OEM = Manufacturer.Apple, Status = ""}
        ];
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterSR = "22";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual.Length).IsEqualTo(2);
        await Assert.That(actual[0].SR).IsEqualTo(phones[1].Ticket.ToString());
        await Assert.That(actual[1].SR).IsEqualTo(phones[3].Ticket.ToString());
    }

    [Test]
    public async Task ChangingFilterStatus_ChangesFilterViewAsync()
    {
        List<Phone> phones = new List<Phone>() {
            new Phone() { Imei = "1", Status="Production", Model = "", Condition = "", OEM = Manufacturer.Apple},
            new Phone() { Imei = "2", Status="In Stock", Model = "", Condition = "", OEM = Manufacturer.Apple},
            new Phone() { Imei = "3", Status="In Repair", Model = "", Condition = "", OEM = Manufacturer.Apple}
        };
        PhonesMainViewModel vm = ViewModelMockSetup(phones);
        await vm.LoadAsync();
        ICollectionView view = CollectionViewSource.GetDefaultView(vm.PhoneItems);

        vm.FilterStatus = "stock";

        var actual = view.OfType<PhonesItemViewModel>().ToArray();
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0].Status).IsEqualTo(phones[1].Status);
    }

    [Test]
    public async Task LoadAsync_should_call_GetActivePhones_when_IncludeDisposals_false()
    {
        List<Phone> phones = [
            new Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = "In Stock"},
            new Phone() { Imei = "2" , AssetTag = "Tag B2", Model = "", Condition = "", OEM = Manufacturer.Samsung, Status = "In Repair"},
            new Phone() { Imei = "3" , AssetTag = "Tag C3", Model = "", Condition = "", OEM = Manufacturer.Nokia, Status = "Production"},
        ];
        AutoMocker mocker = new();
        Mock<IApplicationSettingsRepository> settings = mocker.GetMock<IApplicationSettingsRepository>();
        settings.Setup(s => s.ApplicationSettings).Returns(new ApplicationSettings());
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.GetActivePhonesAsync()).ReturnsAsync(() => { return phones; });
        repository.Setup(r => r.GetAllPhonesAsync()).ReturnsAsync(() => { return phones; });
        Mock<IBaseReportRepository> baseReport = mocker.GetMock<IBaseReportRepository>();
        Mock<IMessenger> messenger = mocker.GetMock<IMessenger>();
        Mock<IPhonesItemViewModelFactory> factory = mocker.GetMock<IPhonesItemViewModelFactory>();
        factory.Setup(r => r.Create(It.IsAny<Phone>()))
                            .Returns((Phone p) => new PhonesItemViewModel(settings.Object, baseReport.Object, repository.Object, messenger.Object, p));
        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();
        //vm.IncludeDisposals = false;
        
        await vm.LoadAsync(); //When IncludeDisposals is false, LoadAsync is not called by OnPropertyChanged, so we need to call it manually here to test the correct repository method is called.

        repository.Verify(r => r.GetActivePhonesAsync(), Times.Once());
        repository.Verify(r => r.GetAllPhonesAsync(), Times.Never());
    }

    [Test]
    public async Task LoadAsync_should_call_GetAllPhones_when_IncludeDisposals_true()
    {
        List<Phone> phones = [
            new Phone() { Imei = "1" , AssetTag = "Tag A1", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = "In Stock"},
            new Phone() { Imei = "2" , AssetTag = "Tag B2", Model = "", Condition = "", OEM = Manufacturer.Samsung, Status = "In Repair"},
            new Phone() { Imei = "3" , AssetTag = "Tag C3", Model = "", Condition = "", OEM = Manufacturer.Nokia, Status = "Production"},
            new Phone() { Imei = "4" , AssetTag = "Tag D4", Model = "", Condition = "", OEM = Manufacturer.Other, Status = "Decommissioned"},
            new Phone() { Imei = "5" , AssetTag = "Tag E5", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = "Disposed"},
        ];
        AutoMocker mocker = new();
        Mock<IApplicationSettingsRepository> settings = mocker.GetMock<IApplicationSettingsRepository>();
        settings.Setup(s => s.ApplicationSettings).Returns(new ApplicationSettings());
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.GetActivePhonesAsync()).ReturnsAsync(() => { return phones; });
        repository.Setup(r => r.GetAllPhonesAsync()).ReturnsAsync(() => { return phones; });
        Mock<IBaseReportRepository> baseReport = mocker.GetMock<IBaseReportRepository>();
        Mock<IMessenger> messenger = mocker.GetMock<IMessenger>();
        Mock<IPhonesItemViewModelFactory> factory = mocker.GetMock<IPhonesItemViewModelFactory>();
        factory.Setup(r => r.Create(It.IsAny<Phone>()))
                            .Returns((Phone p) => new PhonesItemViewModel(settings.Object, baseReport.Object, repository.Object, messenger.Object, p));
        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();

        vm.IncludeDisposals = true;

        await Task.Delay(1000); //Wait for the async LoadAsync to complete before verifying, as it is called by OnPropertyChanged when IncludeDisposals is set to true.
        repository.Verify(r => r.GetAllPhonesAsync(), Times.Once);
        repository.Verify(r => r.GetActivePhonesAsync(), Times.Never());
    }

    [Test]
    public async Task Receive_Phone_should_add_phone_to_PhoneItems()
    {
        AutoMocker mocker = new();
        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();

        vm.Receive(new Phone() { Imei = "1", AssetTag = "Tag A1", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = "" });

        await Assert.That(vm.PhoneItems.Any()).IsTrue();
    }

    [Test]
    public async Task Receive_ProductionPhoneWarning_should_make_warning_visible()
    {
        AutoMocker mocker = new();
        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();

        vm.Receive(new ProductionPhoneWarning("message"));

        await Assert.That(vm.ProductionPhoneWarning).IsEqualTo(Visibility.Visible);
    }

    [Test]
    public async Task SelectedPhone_should_leave_ConcurrentUpdateWarning_as_collapsed_when_ConcurrentChange_false()
    {
        AutoMocker mocker = new();
        Mock<IApplicationSettingsRepository> settings = mocker.GetMock<IApplicationSettingsRepository>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.ConcurrentChange("1", "lastupdate")).ReturnsAsync(false);
        Mock<IBaseReportRepository> baseReport = mocker.GetMock<IBaseReportRepository>();
        Mock<IMessenger> messenger = mocker.GetMock<IMessenger>();
        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();

        await Assert.That(vm.ConcurrentUpdateWarning).IsEqualTo(Visibility.Collapsed);
        vm.SelectedPhone = new PhonesItemViewModel(settings.Object, baseReport.Object, repository.Object, messenger.Object,
            new Phone() { Imei = "1", AssetTag = "Tag A1", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = "In Stock", LastUpdate = "lastupdate" });

        repository.Verify(r => r.ConcurrentChange("1", "lastupdate"), Times.Once());
        await Assert.That(vm.ConcurrentUpdateWarning).IsEqualTo(Visibility.Collapsed);
    }

    [Test]
    public async Task SelectedPhone_should_set_ConcurrentUpdateWarning_visible_when_ConcurrentChange_true()
    {
        AutoMocker mocker = new();
        Mock<IApplicationSettingsRepository> settings = mocker.GetMock<IApplicationSettingsRepository>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.ConcurrentChange("1", "lastupdate")).ReturnsAsync(true);
        Mock<IBaseReportRepository> baseReport = mocker.GetMock<IBaseReportRepository>();
        Mock<IMessenger> messenger = mocker.GetMock<IMessenger>();
        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();

        await Assert.That(vm.ConcurrentUpdateWarning).IsEqualTo(Visibility.Collapsed);
        vm.SelectedPhone = new PhonesItemViewModel(settings.Object, baseReport.Object, repository.Object, messenger.Object,
            new Phone() { Imei = "1", AssetTag = "Tag A1", Model = "", Condition = "", OEM = Manufacturer.Apple, Status = "In Stock", LastUpdate="lastupdate" });

        repository.Verify(r => r.ConcurrentChange("1", "lastupdate"), Times.Once());
        await Assert.That(vm.ConcurrentUpdateWarning).IsEqualTo(Visibility.Visible);
    }

    private static PhonesMainViewModel ViewModelMockSetup(List<Phone> phones)
    {
        int index = 0;
        AutoMocker mocker = new();

        Mock<IApplicationSettingsRepository> settings = mocker.GetMock<IApplicationSettingsRepository>();
        Mock<IPhonesRepository> repository = mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.GetActivePhonesAsync()).ReturnsAsync(phones);
        repository.Setup(r => r.GetAllPhonesAsync()).ReturnsAsync(phones);
        Mock<IBaseReportRepository> baseReport = mocker.GetMock<IBaseReportRepository>();
        Mock<IMessenger> messenger = mocker.GetMock<IMessenger>();
        Mock<IPhonesItemViewModelFactory> factory = mocker.GetMock<IPhonesItemViewModelFactory>();
        factory.Setup(r => r.Create(It.IsAny<Phone>()))
                            .Returns(() => new PhonesItemViewModel(settings.Object, baseReport.Object, repository.Object, messenger.Object, phones[index]))
                            .Callback(() => index++);

        PhonesMainViewModel vm = mocker.CreateInstance<PhonesMainViewModel>();
        return vm;
    }
}
