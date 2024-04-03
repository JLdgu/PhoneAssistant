using Moq.AutoMock;

using Xunit;
using PhoneAssistant.WPF.Features.AddItem;
using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.Tests.Features.AddItem;
public class AddItemViewModelTests
{
    private readonly AutoMocker _mocker = new AutoMocker();
    //private readonly Mock<IPhonesRepository> _phones;

    private readonly AddItemViewModel _sut;

    public AddItemViewModelTests()
    {
        //_phones = _mocker.GetMock<IPhonesRepository>();

        _sut = _mocker.CreateInstance<AddItemViewModel>();
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
    void PhoneClearCommand_ShouldDisablePhoneSave()
    {
        _sut.PhoneClearCommand.Execute(null);

        Assert.False(_sut.CanSavePhone());        
    }

    [Fact]
    void PhoneSaveCommand_ShouldResetAllProperties()
    {
        _sut.PhoneClearCommand.Execute(null);

        Assert.Equal(string.Empty, _sut.PhoneCondition);
        Assert.Equal(string.Empty, _sut.PhoneStatus);
        Assert.Equal(string.Empty, _sut.PhoneImei);
    }

    [Fact]
    void PhoneSaveCommand_ShouldDisablePhoneSave()
    {
        _sut.PhoneClearCommand.Execute(null);

        Assert.False(_sut.CanSavePhone());
    }

    [Fact]
    void PhoneWithValidProperties_ShouldEnableSave()
    {
        _sut.PhoneCondition = ApplicationSettings.Conditions[0];

        Assert.True(_sut.CanSavePhone());
    }


}
