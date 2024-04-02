using Moq.AutoMock;

using Xunit;
using PhoneAssistant.WPF.Features.AddItem;

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

        Assert.False(_sut.ConditionNew);
        Assert.False(_sut.ConditionRepurposed);
    }
}
