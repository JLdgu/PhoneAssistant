using ClosedXML.Excel;
using FluentResults;
using Serilog;

namespace PhoneAssistant.Cli;

public record ExcelRow(string Name, int Certificate);

public sealed class DisposalExport
{
    private readonly List<SccDisposal> _disposals;
    private readonly List<Device> _devices;
    private readonly XLWorkbook _disposalsWorkbook;
    private readonly XLWorkbook _notesWorkbook;
    private readonly string _disposalFile;
    private readonly string _notesFile;

    public IXLWorksheet DisposalsSheet { get; private set; }
    public IXLWorksheet NotesSheet { get; private set; }

    public static (int, string) Name { get => (0, "Name"); }
    public static (int, string) Owner { get => (1, "Owner"); }
    public static (int, string) Status { get => (2, "Status"); }
    public static (int, string) SubLocation { get => (3, "Sub-Location"); }
    public static (int, string) Notes { get => (1, "CI Notes"); }
    public int RowCount { get; private set; }

    public DisposalExport(List<SccDisposal> disposals, List<Device> devices, DirectoryInfo exportDirectory)
    {
        _disposals = disposals;
        _devices = devices;
        _disposalsWorkbook = new XLWorkbook();
        DisposalsSheet = _disposalsWorkbook.Worksheets.Add("Disposals");
        _disposalFile = Path.Combine(exportDirectory.FullName, $"Disposal_{DateTime.Now:yyyyMMdd_Hmmss}.xlsx");
        _notesWorkbook = new XLWorkbook();
        NotesSheet = _notesWorkbook.Worksheets.Add("Notes");
        _notesFile = Path.Combine(exportDirectory.FullName, $"DisposalNotes_{DateTime.Now:yyyyMMdd_Hmmss}.xlsx");
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
            _disposalsWorkbook.SaveAs(_disposalFile);
            Log.Information("{0} created", _disposalFile);
            _notesWorkbook.SaveAs(_notesFile);
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
            var disposalHeader = DisposalsSheet.Row(1);
            disposalHeader.Cell(Name.Item1 + 1).Value = Name.Item2;
            disposalHeader.Cell(Owner.Item1 + 1).Value = Owner.Item2;
            disposalHeader.Cell(Status.Item1 + 1).Value = Status.Item2;
            disposalHeader.Cell(SubLocation.Item1 + 1).Value = SubLocation.Item2;

            var notesHeader = NotesSheet.Row(1);
            notesHeader.Cell(Name.Item1 + 1).Value = Name.Item2;
            notesHeader.Cell(Notes.Item1 + 1).Value = Notes.Item2;
        }

        RowCount++;
        var disposalRow = DisposalsSheet.Row(RowCount + 1);
        disposalRow.Cell(Name.Item1 + 1).Value = disposal.Name;
        disposalRow.Cell(Owner.Item1 + 1).Value = "";
        disposalRow.Cell(Status.Item1 + 1).Value = "Disposed";
        disposalRow.Cell(SubLocation.Item1 + 1).Value = "";

        var notesRow = NotesSheet.Row(RowCount + 1);
        notesRow.Cell(Name.Item1 + 1).Value = disposal.Name;
        notesRow.Cell(Notes.Item1 + 1).Value = $"SCC Certificate # {disposal.Certificate}";
    }
}
