using FluentResults;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Serilog;

namespace Reconcile;

public record ExcelRow(string Name, int Certificate);

public sealed class Export
{
    private readonly List<Disposal> _disposals;
    private readonly List<Device> _devices;
    private readonly IWorkbook _workbook;
    private readonly string _fileName;

    public ISheet Sheet { get; private set; }
    public static (int, string) Name { get => (0, "Name"); }
    public static (int, string) Notes { get => (1, "CI Notes"); }
    public static (int, string) Owner { get => (2, "Owner"); }
    public static (int, string) Status { get => (3, "CI Status"); }
    public static (int, string) SubLocation { get => (4, "Sub-Location"); }
    public int RowCount { get; private set; }

    public Export(List<Disposal> disposals, List<Device> devices, DirectoryInfo exportDirectory)
    {
        _disposals = disposals;
        _devices = devices;
        _workbook = new XSSFWorkbook();
        Sheet = _workbook.CreateSheet("Disposals");
        _fileName = Path.Combine(exportDirectory.FullName, $"Disposals{DateTime.Now:yyMMdd_Hmmss}.xlsx");
    }

    public Result Execute()
    {
        foreach (Disposal disposal in _disposals)
        {
            Result<ExcelRow> result = GetDevice(disposal);
            if (result.IsSuccess) AddRow(result.Value);
        }

        Log.Information("{0} CIs to be amended", RowCount);
        Log.Information("{0} created",_fileName);

        if (RowCount > 0)
        {
            using var fs = new FileStream(_fileName, FileMode.Create, FileAccess.Write);
            _workbook.Write(fs);
        }

        return Result.Ok();
    }

    public Result<ExcelRow> GetDevice(Disposal disposal)
    {
        if (_devices.Any(d => d.Name == disposal.PrimaryKey && d.Status != "Disposed"))
            return Result.Ok(new ExcelRow(disposal.PrimaryKey, disposal.Certificate));
        if(_devices.Any(d => d.Name == disposal.SecondaryKey && d.Status != "Disposed"))
            return Result.Ok(new ExcelRow(disposal.SecondaryKey!, disposal.Certificate));

        if(_devices.Any(d => d.AssetTag == disposal.PrimaryKey && d.Status != "Disposed"))
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
            IRow header = Sheet.CreateRow(0);
            header.CreateCell(Name.Item1).SetCellValue(Name.Item2);
            header.CreateCell(Notes.Item1).SetCellValue(Notes.Item2);
            header.CreateCell(Owner.Item1).SetCellValue(Owner.Item2);
            header.CreateCell(Status.Item1).SetCellValue(Status.Item2);
            header.CreateCell(SubLocation.Item1).SetCellValue(SubLocation.Item2);
        }

        RowCount++;
        IRow row = Sheet.CreateRow(RowCount);
        row.CreateCell(Name.Item1).SetCellValue(disposal.Name);
        row.CreateCell(Notes.Item1).SetCellValue($"SCC Certificate # {disposal.Certificate}");
        row.CreateCell(Owner.Item1).SetCellValue("");
        row.CreateCell(Status.Item1).SetCellValue("Disposed");
        row.CreateCell(SubLocation.Item1).SetCellValue("");
    }

}
