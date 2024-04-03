using Moq.AutoMock;

using Xunit;
using PhoneAssistant.WPF.Features.AddItem;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Repositories;
using Moq;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Phones;

namespace PhoneAssistant.Tests.Features.AddItem;
public class AddItemViewModelTests
{
    private readonly AutoMocker _mocker = new AutoMocker();
    private readonly AddItemViewModel _sut;

    public AddItemViewModelTests()
    {
        _sut = _mocker.CreateInstance<AddItemViewModel>();
    }

    [Fact]
    void PhoneClearCommand_ShouldDisablePhoneSave()
    {
        _sut.PhoneClearCommand.Execute(null);

        Assert.False(_sut.CanSavePhone());
    }

    [Fact]
    void PhoneClearCommand_ShouldResetAllProperties()
    {
        _sut.PhoneClearCommand.Execute(null);

        Assert.Equal(string.Empty, _sut.PhoneCondition); 
        Assert.Equal(string.Empty, _sut.PhoneStatus);
        Assert.Equal(string.Empty,_sut.PhoneImei);
    }

    [Fact]
    void PhoneSaveCommand_ShouldDisablePhoneSave()
    {
        _sut.PhoneSaveCommand.Execute(null);

        Assert.False(_sut.CanSavePhone());
    }

    [Fact]
    void PhoneSaveCommand_ShouldResetAllProperties()
    {
        _sut.PhoneSaveCommand.Execute(null);

        Assert.Equal(string.Empty, _sut.PhoneCondition);
        Assert.Equal(string.Empty, _sut.PhoneStatus);
        Assert.Equal(string.Empty, _sut.PhoneImei);
    }

    [Fact]
    void PhoneSaveCommand_ShouldSaveChanges()
    {        
        Mock<IPhonesRepository> repository = _mocker.GetMock<IPhonesRepository>();
        //repository.Setup()            .ReturnsAsync("LastUpdate");


        _sut.PhoneSaveCommand.Execute(null);        
    }

    [Fact]
    void PhoneWithValidProperties_ShouldEnableSave()
    {
        _sut.PhoneCondition = ApplicationSettings.Conditions[0];
        _sut.PhoneStatus = ApplicationSettings.Statuses[0];

        Assert.True(_sut.CanSavePhone());
    }


}
