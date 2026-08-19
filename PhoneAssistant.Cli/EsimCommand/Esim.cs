using System.CommandLine;

using PhoneAssistant.Model;

using Serilog;

namespace PhoneAssistant.Cli.EsimCommand;

internal static class Esim
{
    internal static void Command(RootCommand rootCommand)
    {
        Command esimCommand = new("esim", "Ensure eSim history is up to date");

        esimCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            try
            {
                var executor = new EsimExecutor();
                await EsimExecutor.Execute();
            }
            catch (Exception ex)
            {
                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        });

        rootCommand.Add(esimCommand);
    }
}

public sealed class EsimExecutor
{
    public static async Task Execute()
    {
        PhoneAssistantDbContext dbContext = ModelContext.Create();
        Log.Information("eSim history update started");

        SimRepository repository = new(dbContext);
        IEnumerable<Tuple<string,string>> phoneNumbers = await repository.GetEsims();
        if (!phoneNumbers.Any())
        {
            Log.Error("No eSims found.");
            return;
        }

        await CheckAndUpdateHistory(repository, phoneNumbers);
        dbContext.SaveChanges();
    }

    public static async Task CheckAndUpdateHistory(ISimRepository repository, IEnumerable<Tuple<string,string>> phoneNumbers)
    {
        int updateCount = 0;
        foreach (Tuple<string,string> phoneNumber in phoneNumbers)
        {
            IEnumerable<Sim> sims = await repository.GetPhysicalSimsForPhoneNumberAndSimNumber(phoneNumber.Item1, phoneNumber.Item2);
            if (!sims.Any())
            {
                continue;
            }
            foreach (Sim sim in sims)
            {
                sim.Esim = true;
                await repository.UpdateOrCreateAsync(sim);
                updateCount++;
            }
        }
        Log.Information("eSim history update completed. {UpdateCount} records updated.", updateCount);
    }
}
