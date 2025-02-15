﻿using System;
using System.ComponentModel;

using EnumsNET;

using FluentAssertions;
using FluentResults;

using Xunit.Abstractions;

namespace Reconcile.Tests;

public sealed class ExportTests()
{
    [Fact]
    public void AddRow_ShouldAddHeaderAndDataRow_WhenFirstRowAdded()
    {
        var export = new Export(0,[], [], new DirectoryInfo("c:"));
        
        export.AddRow(new("name", 5));

        export.RowCount.Should().Be(1);
        var header = export.DisposalsSheet.GetRow(0);
        header.GetCell(Export.Name.Item1).StringCellValue.Should().Be(Export.Name.Item2);    
        header.GetCell(Export.Owner.Item1).StringCellValue.Should().Be(Export.Owner.Item2);
        header.GetCell(Export.Status.Item1).StringCellValue.Should().Be(Export.Status.Item2);
        header.GetCell(Export.SubLocation.Item1).StringCellValue.Should().Be(Export.SubLocation.Item2);
        var actualRow = export.DisposalsSheet.GetRow(1);
        actualRow.GetCell(Export.Name.Item1).StringCellValue.Should().Be("name");
        actualRow.GetCell(Export.Owner.Item1).StringCellValue.Should().Be("");
        actualRow.GetCell(Export.Status.Item1).StringCellValue.Should().Be("Disposed");
        actualRow.GetCell(Export.SubLocation.Item1).StringCellValue.Should().Be("");

        var notesHeader = export.NotesSheet.GetRow(0);
        notesHeader.GetCell(Export.Name.Item1).StringCellValue.Should().Be(Export.Name.Item2);
        notesHeader.GetCell(Export.Notes.Item1).StringCellValue.Should().Be(Export.Notes.Item2);
        var notesRow = export.NotesSheet.GetRow(1);
        notesRow.GetCell(Export.Name.Item1).StringCellValue.Should().Be("name");
        notesRow.GetCell(Export.Notes.Item1).StringCellValue.Should().Be($"SCC Certificate # 5");
    }

    [Fact]
    public void AddRow_ShouldOnlyAddDataRow_WhenSubsequentRowsAdded()
    {
        var export = new Export(0,[], [], new DirectoryInfo("c:"));
        export.AddRow(new("name", 6));
        export.AddRow(new("name2", 7));

        export.RowCount.Should().Be(2);
        var actualRow = export.DisposalsSheet.GetRow(2);
        actualRow.GetCell(Export.Name.Item1).StringCellValue.Should().Be("name2");
        actualRow.GetCell(Export.Owner.Item1).StringCellValue.Should().Be("");
        actualRow.GetCell(Export.Status.Item1).StringCellValue.Should().Be("Disposed");
        actualRow.GetCell(Export.SubLocation.Item1).StringCellValue.Should().Be("");
        var notesRow = export.NotesSheet.GetRow(2);
        notesRow.GetCell(Export.Name.Item1).StringCellValue.Should().Be("name2");
        notesRow.GetCell(Export.Notes.Item1).StringCellValue.Should().Be($"SCC Certificate # 7");
    }

    [Theory]
    [MemberData(nameof(NotFoundOrStatusDisposedTestData))]
    public void GetDevice_ShouldFail_WhenNotFoundOrStatusDisposed(Disposal disposal,string status)
    {
        var disposals = new List<Disposal>();
        var devices = new List<Device>() { new("name", "assetTag", "serialNumber", status) };
        var export = new Export(0,disposals, devices, new DirectoryInfo("c:"));

        Result<ExcelRow> result = export.GetDevice(disposal);

        result.IsFailed.Should().BeTrue();
    }
    public static TheoryData<Disposal, string> NotFoundOrStatusDisposedTestData =>    
        new()
        {
            { new Disposal("notfound",null, 0), ""},
            { new Disposal("notfound","notfound", 1), "" },
            { new Disposal("name",null, 2), "Disposed" },
            { new Disposal("assetTag",null, 3), "Disposed" },
            { new Disposal("serialNumber",null, 4), "Disposed" },
            { new Disposal("notfound","name", 5), "Disposed" },
            { new Disposal("notfound","assetTag", 6), "Disposed" },
            { new Disposal("notfound","serialNumber", 7), "Disposed" }
        };

    [Theory]
    [MemberData(nameof(FoundTestData))]
    public void GetDevice_ShouldSucceed_WhenFound(Disposal disposal)
    {
        var disposals = new List<Disposal>();
        var devices = new List<Device>() { new("name", "assetTag", "serialNumber", "status") };
        var export = new Export(0,disposals, devices, new DirectoryInfo("c:"));

        Result<ExcelRow> result = export.GetDevice(disposal);

        result.IsSuccess.Should().BeTrue();
        result.Value.Certificate.Should().Be(disposal.Certificate);
    }
    public static TheoryData<Disposal> FoundTestData =>
        new()
        {
            { new Disposal("name",null, 1) },
            { new Disposal("assetTag",null, 2) },
            { new Disposal("serialNumber",null, 3) },
            { new Disposal("notfound","name", 4) },
            { new Disposal("notfound","assetTag", 5) },
            { new Disposal("notfound","serialNumber", 6) }
        };
}
