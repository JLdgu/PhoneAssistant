using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Dashboard;

public interface IPrintEnvelope
{
    void Execute(v1Phone? phone);
}