﻿using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

using Xunit;

namespace PhoneAssistant.Tests.Application.Repositories;
public class ImportHistoryRepositoryTests
{
    readonly DbTestHelper _helper = new();
    readonly ImportHistoryRepository _repository;

    public ImportHistoryRepositoryTests()
    {
        _repository = new(_helper.DbContext);
    }

    [Fact]
    async Task GetLatestImport_WhenImportExists_ShouldReturnLatest()
    {
        List<ImportHistory> imports = new() { 
            new() { Name = ImportType.BaseReport, File = "Import 1", ImportDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") },
            new() { Name = ImportType.BaseReport, File = "Import 2", ImportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }        };

        _helper.DbContext.Imports.AddRange(imports);
        await _helper.DbContext.SaveChangesAsync();

        ImportHistory? actual = _repository.GetLatestImport();

        Assert.Equal(imports[1], actual);
    }
}
