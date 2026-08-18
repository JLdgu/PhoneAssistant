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
        IEnumerable<string> phoneNumbers = await repository.GetEsims();
        if (!phoneNumbers.Any())
        {
            Log.Error("No eSims found.");
            return;
        }

        await CheckAndUpdateHistory(repository, phoneNumbers);
        dbContext.SaveChanges();
    }

    public static async Task CheckAndUpdateHistory(ISimRepository repository, IEnumerable<string> phoneNumbers)
    {
        foreach (string phoneNumber in phoneNumbers)
        {
            IEnumerable<Sim> sims = await repository.GetSimsForPhoneNumber(phoneNumber);
            if (!sims.Any())
            {
                Log.Error("No eSim history found for phone number {PhoneNumber}.", phoneNumber);
                continue;
            }
            string simNumber = string.Empty;
            bool updateRequired = false;
            Log.Information("Updating eSim history for phone number {PhoneNumber}", phoneNumber);
            foreach (Sim sim in sims)
            {
                if (sim.Esim == true)
                {
                    simNumber = sim.SIMNumber;
                    updateRequired = true;
                    continue;
                }

                if (updateRequired && sim.SIMNumber == simNumber)
                {
                    sim.Esim = true;
                    await repository.UpdateOrCreateAsync(sim);
                    Log.Information("Updated eSim history {PhoneNumber} : {BillingPeriod}", phoneNumber, sim.BillingPeriod);
                }
            }
        }
    }
}
