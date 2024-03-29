﻿using Moq;
using Moq.AutoMock;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Features.Disposals;

using Xunit;

namespace PhoneAssistant.Tests.Features.Disposals;

public sealed class ReconciliationTests
{
    //    private readonly AutoMocker _mocker = new AutoMocker();
    //    readonly Reconciliation _iut;
    //    readonly Mock<IDisposalsRepository> _repository;
    //    Disposal _actual = new() { Imei = "" };

    //    public ReconciliationTests()
    //    {
    //        _iut = _mocker.CreateInstance<Reconciliation>();
    //        _repository = _mocker.GetMock<IDisposalsRepository>();
    //    }

    //    #region DCC
    //    [Fact]
    //    private async Task Disposal_DCC_WithMissingDisposal_AddsDisposal()
    //    {
    //        _repository.Setup(r => r.GetDisposalAsync("imei")).ReturnsAsync(() => null);
    //        _repository.Setup(r => r.AddAsync(It.IsAny<Disposal>()))
    //            .Callback<Disposal>((d) => _actual = d);

    //        Result result = await _iut.DisposalAsync(Import.DCC, "imei", "status");

    //        _repository.VerifyAll();
    //        Assert.Equal(Result.Added, result);
    //        Assert.Equal("status", _actual.StatusMS);
    //        Assert.Null(_actual.StatusPA);
    //        Assert.Null(_actual.StatusSCC);
    //        Assert.Null(_actual.Action);
    //    }

    //    [Fact]
    //    private async Task Disposal_DCC_WithMismatchedStatus_UpdatesStatus()
    //    {
    //        _repository.Setup(r => r.GetDisposalAsync("imei")).ReturnsAsync(new Disposal() { Imei = "imei", StatusMS = "status" });
    //        _repository.Setup(r => r.UpdateAsync(It.IsAny<Disposal>()))
    //            .Callback<Disposal>((d) => _actual = d);

    //        Result result = await _iut.DisposalAsync(Import.DCC, "imei", "newstatus");

    //        _repository.VerifyAll();
    //        Assert.Equal(Result.Updated, result);
    //        Assert.Equal("newstatus", _actual.StatusMS);
    //        Assert.Null(_actual.StatusPA);
    //        Assert.Null(_actual.StatusSCC);
    //        Assert.Null(_actual.Action);
    //    }

    //    [Fact]
    //    private async Task Disposal_DCC_WithMatchingStatus_ReturnsUnchanged()
    //    {
    //        Disposal disposal = new() { Imei = "imei", StatusMS = "DCC" };
    //        _repository.Setup(r => r.GetDisposalAsync("imei")).ReturnsAsync(disposal);

    //        Result result = await _iut.DisposalAsync(Import.DCC, "imei", "DCC");

    //        _repository.VerifyAll();
    //        Assert.Equal(Result.Unchanged, result);
    //        Assert.Equal("DCC", disposal.StatusMS);
    //        Assert.Null(_actual.StatusPA);
    //        Assert.Null(_actual.StatusSCC);
    //        Assert.Null(_actual.Action);
    //    }
    //    #endregion

    //    #region PA
    //    [Fact]
    //    private async Task Disposal_PA_WithMissingDisposal_AddsDisposal()
    //    {
    //        _repository.Setup(r => r.GetDisposalAsync("imei")).ReturnsAsync(() => null);
    //        _repository.Setup(r => r.AddAsync(It.IsAny<Disposal>()))
    //            .Callback<Disposal>((d) => _actual = d);

    //        Result result = await _iut.DisposalAsync(Import.PA, "imei", "status");

    //        _repository.VerifyAll();
    //        Assert.Equal(Result.Added, result);
    //        Assert.Null(_actual.StatusMS);
    //        Assert.Equal("status", _actual.StatusPA);
    //        Assert.Null(_actual.StatusSCC);
    //        Assert.Equal("Missing from myScomis", _actual.Action);
    //    }

    //    [Fact]
    //    private async Task Disposal_PA_WithMismatchedStatus_UpdatesStatus()
    //    {
    //        _repository.Setup(r => r.GetDisposalAsync("imei")).ReturnsAsync(new Disposal() { Imei = "imei", StatusPA = "status" });
    //        _repository.Setup(r => r.UpdateAsync(It.IsAny<Disposal>()))
    //            .Callback<Disposal>((d) => _actual = d);

    //        Result result = await _iut.DisposalAsync(Import.PA, "imei", "newstatus");

    //        _repository.VerifyAll();
    //        Assert.Equal(Result.Updated, result);
    //        Assert.Null(_actual.StatusMS);
    //        Assert.Equal("newstatus", _actual.StatusPA);
    //        Assert.Null(_actual.StatusSCC);
    //        Assert.Null(_actual.Action);
    //    }

    //    [Fact]
    //    private async Task Disposal_PA_WithMatchingStatus_ReturnsUnchanged()
    //    {
    //        Disposal disposal = new() { Imei = "imei", StatusPA = "PA" };
    //        _repository.Setup(r => r.GetDisposalAsync("imei")).ReturnsAsync(disposal);

    //        Result result = await _iut.DisposalAsync(Import.PA, "imei", "PA");

    //        _repository.VerifyAll();
    //        Assert.Equal(Result.Unchanged, result);
    //        Assert.Null(_actual.StatusMS);
    //        Assert.Equal("PA", disposal.StatusPA);
    //        Assert.Null(_actual.StatusSCC);
    //        Assert.Null(_actual.Action);
    //    }
    //    #endregion

    //    #region SCC
    //    [Fact]
    //    private async Task Disposal_SCC_WithMissingDisposal_AddsDisposal()
    //    {
    //        _repository.Setup(r => r.GetDisposalAsync("imei")).ReturnsAsync(() => null);
    //        _repository.Setup(r => r.AddAsync(It.IsAny<Disposal>()))
    //            .Callback<Disposal>((d) => _actual = d);

    //        Result result = await _iut.DisposalAsync(Import.SCC, "imei", "status");

    //        _repository.VerifyAll();
    //        Assert.Equal(Result.Added, result);
    //        Assert.Null(_actual.StatusMS);
    //        Assert.Null(_actual.StatusPA);
    //        Assert.Equal("status", _actual.StatusSCC);
    //        Assert.Equal("Missing from myScomis", _actual.Action);
    //    }

    //    [Fact]
    //    private async Task Disposal_SCC_WithMismatchedStatus_UpdatesStatus()
    //    {
    //        _repository.Setup(r => r.GetDisposalAsync("imei")).ReturnsAsync(new Disposal() { Imei = "imei", StatusPA = "status" });
    //        _repository.Setup(r => r.UpdateAsync(It.IsAny<Disposal>()))
    //            .Callback<Disposal>((d) => _actual = d);

    //        Result result = await _iut.DisposalAsync(Import.SCC, "imei", "newstatus");

    //        _repository.VerifyAll();
    //        Assert.Equal(Result.Updated, result);
    //        Assert.Null(_actual.StatusMS);
    //        Assert.Equal("newstatus", _actual.StatusPA);
    //        Assert.Null(_actual.StatusSCC);
    //        Assert.Null(_actual.Action);
    //    }

    //    [Fact]
    //    private async Task Disposal_SCC_WithMatchingStatus_ReturnsUnchanged()
    //    {
    //        Disposal disposal = new() { Imei = "imei", StatusPA = "PA" };
    //        _repository.Setup(r => r.GetDisposalAsync("imei")).ReturnsAsync(disposal);

    //        Result result = await _iut.DisposalAsync(Import.SCC, "imei", "PA");

    //        _repository.VerifyAll();
    //        Assert.Equal(Result.Unchanged, result);
    //        Assert.Null(_actual.StatusMS);
    //        Assert.Equal("PA", disposal.StatusPA);
    //        Assert.Null(_actual.StatusSCC);
    //        Assert.Null(_actual.Action);
    //    }
    //    #endregion
}
