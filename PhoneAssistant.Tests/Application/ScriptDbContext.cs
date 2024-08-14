using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

using Xunit;
using Xunit.Abstractions;

namespace PhoneAssistant.Tests.Application;

public class ScriptDBContext(ITestOutputHelper output)
{

    [Fact]    
    [Trait("DBContext","Script")]
    public void GenerateSQLScript()
    {
        
        DbTestHelper helper = new();

        string sql = helper.DbContext.Database.GenerateCreateScript();

        File.WriteAllText("c:/temp/DbCreate.sql", sql);
    }

    [Fact]
    public async Task SimUpdateAsync()
    {
        DbTestHelper helper = new(@"DataSource=c:\dev\patest.db;");
        //DbTestHelper helper = new(@"DataSource=K:/FITProject/ICTS/Mobile Phones/PhoneAssistant/PhoneAssistant.db");

        List<Sim> sims = await helper.DbContext.Sims.ToListAsync();

        foreach (Sim sim in sims)
        {
            EEBaseReport? baseSim = await helper.DbContext.BaseReport.FindAsync(sim.PhoneNumber);
            if (baseSim == null)
            {
                output.WriteLine("Not found {0}", sim.PhoneNumber);
                continue;
            }

            if (baseSim.SIMNumber == sim.SimNumber)
                continue;

            output.WriteLine("Reconciling {0} {1} {2}", sim.PhoneNumber, sim.SimNumber, baseSim.SIMNumber);

            sim.SimNumber = baseSim.SIMNumber;
            helper.DbContext.Sims.Update(sim);
            helper.DbContext.SaveChanges();

            //Sim? dupSim = await helper.DbContext.Sims.FirstOrDefaultAsync(s => s.SimNumber == baseSim.SIMNumber && s.PhoneNumber != baseSim.PhoneNumber);
            //if (dupSim == null) continue;

            //output.WriteLine("Delete {0}", dupSim.PhoneNumber);
        }
    }
}