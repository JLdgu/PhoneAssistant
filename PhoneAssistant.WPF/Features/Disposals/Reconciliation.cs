﻿using MaterialDesignThemes.Wpf;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PhoneAssistant.WPF.Features.Disposals;

public sealed class Reconciliation(IDisposalsRepository disposalsRepository)
{
    private readonly IDisposalsRepository _disposalsRepository = disposalsRepository ?? throw new ArgumentNullException(nameof(disposalsRepository));

    public async Task<Result> DisposalAsync(Import import, string imei, string status)
    {
        Disposal? disposal = await disposalsRepository.GetDisposalAsync(imei);
        if (disposal is null) // Add new disposal
        {
            disposal = new() { Imei = imei };
            switch (import)
            {
                case Import.DCC:
                    disposal.StatusMS = status;
                    break;
                case Import.PA:
                    disposal.StatusPA = status;
                    disposal.Action = "Missing from myScomis";
                    break;
                case Import.SCC: 
                    //disposal.StatusSCC = status;
                    break;
            }

            await disposalsRepository.AddAsync(disposal);
            return Result.Added;
        }

        // Check for unchanged status
        switch (import) 
        {
            case Import.DCC:
                if (disposal.StatusMS == status)
                    return Result.Unchanged;
                break;
            case Import.PA:
                if (disposal.StatusPA == status)
                    return Result.Unchanged;
                break;
            case Import.SCC:
                break;
        }

        // Update with changed status
        switch (import)
        {
            case Import.DCC:
                disposal.StatusMS = status;
                break;
            case Import.PA:
                disposal.StatusPA = status;
                break;
            case Import.SCC:
                //disposal.StatusSCC = status;
                break;
        }
        await disposalsRepository.UpdateAsync(disposal);
        return Result.Updated;
    }
}
