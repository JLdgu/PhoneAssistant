using FluentResults;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Serilog;

namespace PhoneAssistant.Cli;

public record ExcelRow(string Name, int Certificate);

public sealed class DisposalExport
{
    private readonly List<SccDisposal> _disposals;
    private readonly List<Device> _devices;
    private readonly IWorkbook _disposalsWorkbook;
    private readonly IWorkbook _notesWorkbook;
    private readonly string _disposalFile;
    private readonly string _notesFile;

    public ISheet DisposalsSheet { get; private set; }
    public ISheet NotesSheet { get; private set; }

    public static (int, string) Name { get => (0, "Name"); }
    public static (int, string) Owner { get => (1, "Owner"); }
    public static (int, string) Status { get => (2, "Status"); }
    public static (int, string) SubLocation { get => (3, "Sub-Location"); }
    public static (int, string) Notes { get => (1, "CI Notes"); }
    public int RowCount { get; private set; }

    public DisposalExport(int sr, List<SccDisposal> disposals, List<Device> devices, DirectoryInfo exportDirectory)
    {
        _disposals = disposals;
        _devices = devices;
        _disposalsWorkbook = new XSSFWorkbook();
        DisposalsSheet = _disposalsWorkbook.CreateSheet("Disposals");
        _disposalFile = Path.Combine(exportDirectory.FullName, $"SR{sr}_Disposals_{DateTime.Now:yyMMdd_Hmmss}.xlsx");
        _notesWorkbook = new XSSFWorkbook();
        NotesSheet = _notesWorkbook.CreateSheet("Notes");
        _notesFile = Path.Combine(exportDirectory.FullName, $"SR{sr}_DisposalNotes_{DateTime.Now:yyMMdd_Hmmss}.xlsx");
    }

    public Result Execute()
    {
        foreach (SccDisposal disposal in _disposals)
        {
            Result<ExcelRow> result = GetDevice(disposal);
            if (result.IsSuccess) AddRow(result.Value);
        }

        Log.Information("{0} CIs to be amended", RowCount);

        if (RowCount > 0)
        {
            using var fs = new FileStream(_disposalFile, FileMode.Create, FileAccess.Write);
            _disposalsWorkbook.Write(fs);
            Log.Information("{0} created", _disposalFile);
            using var fs2 = new FileStream(_notesFile, FileMode.Create, FileAccess.Write);
            _notesWorkbook.Write(fs2);
            Log.Information("{0} created", _notesFile);
        }

        return Result.Ok();
    }

    public Result<ExcelRow> GetDevice(SccDisposal disposal)
    {
        if (_devices.Any(d => d.Name == disposal.PrimaryKey && d.Status != "Disposed"))
            return Result.Ok(new ExcelRow(disposal.PrimaryKey, disposal.Certificate));
        if (_devices.Any(d => d.Name == disposal.SecondaryKey && d.Status != "Disposed"))
            return Result.Ok(new ExcelRow(disposal.SecondaryKey!, disposal.Certificate));

        if (_devices.Any(d => d.AssetTag == disposal.PrimaryKey && d.Status != "Disposed"))
            return Result.Ok(new ExcelRow(disposal.PrimaryKey!, disposal.Certificate));
        if (_devices.Any(d => d.AssetTag == disposal.SecondaryKey && d.Status != "Disposed"))
            return Result.Ok(new ExcelRow(disposal.SecondaryKey!, disposal.Certificate));

        if (_devices.Any(d => d.SerialNumber == disposal.PrimaryKey && d.Status != "Disposed"))
            return Result.Ok(new ExcelRow(disposal.PrimaryKey!, disposal.Certificate));
        if (_devices.Any(d => d.SerialNumber == disposal.SecondaryKey && d.Status != "Disposed"))
            return Result.Ok(new ExcelRow(disposal.SecondaryKey!, disposal.Certificate)); ;
        return Result.Fail("Device not found");
    }

    public void AddRow(ExcelRow disposal)
    {
        if (RowCount == 0)
        {
            IRow header = DisposalsSheet.CreateRow(0);
            header.CreateCell(Name.Item1).SetCellValue(Name.Item2);
            header.CreateCell(Owner.Item1).SetCellValue(Owner.Item2);
            header.CreateCell(Status.Item1).SetCellValue(Status.Item2);
            header.CreateCell(SubLocation.Item1).SetCellValue(SubLocation.Item2);

            IRow notesHeader = NotesSheet.CreateRow(0);
            notesHeader.CreateCell(Name.Item1).SetCellValue(Name.Item2);
            notesHeader.CreateCell(Notes.Item1).SetCellValue(Notes.Item2);
        }

        RowCount++;
        IRow row = DisposalsSheet.CreateRow(RowCount);
        row.CreateCell(Name.Item1).SetCellValue(disposal.Name);
        row.CreateCell(Owner.Item1).SetCellValue("");
        row.CreateCell(Status.Item1).SetCellValue("Disposed");
        row.CreateCell(SubLocation.Item1).SetCellValue("");

        IRow notesRow = NotesSheet.CreateRow(RowCount);
        notesRow.CreateCell(Name.Item1).SetCellValue(disposal.Name);
        notesRow.CreateCell(Notes.Item1).SetCellValue($"SCC Certificate # {disposal.Certificate}");
    }
}
