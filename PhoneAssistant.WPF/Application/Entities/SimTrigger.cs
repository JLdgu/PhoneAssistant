using EntityFrameworkCore.Triggered;

namespace PhoneAssistant.WPF.Application.Entities;
internal sealed class SimTrigger : IBeforeSaveTrigger<Sim>
{
    public Task BeforeSave(ITriggerContext<Sim> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType != ChangeType.Deleted)
            context.Entity.LastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");          

        return Task.CompletedTask;
    }
}
