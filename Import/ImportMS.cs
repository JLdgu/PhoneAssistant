namespace Import;

internal sealed class ImportMS(ImportDbContext dbContext)
{
    private readonly ImportDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    internal int Count()
    {
        List<BaseReport> report = [.. dbContext.BaseReport];

        //int ct = 0;
        //foreach (BaseReport reportItem in report)
        //{
        //    ct++;
        //}
        return dbContext.BaseReport.Count(); //report.Count;
    }
}


public class BaseReport
{
    public required string PhoneNumber { get; set; }
    public required string UserName { get; set; }
    public required string ContractEndDate { get; set; }
    public required string TalkPlan { get; set; }
    public required string Handset { get; set; }
    public required string SimNumber { get; set; }
    public required string ConnectedIMEI { get; set; }
    public required string LastUsedIMEI { get; set; }
}
