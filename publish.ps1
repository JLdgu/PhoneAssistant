param(
    [string]$Configuration = "Release",
    [bool]$Build = $true,
    [bool]$Pack = $true
)

[xml]$projectFile = Get-Content "PhoneAssistant.WPF\PhoneAssistant.WPF.csproj"
Write-Host $projectFile

$Version = $projectFile.Project.PropertyGroup.Version | Where-Object { $_ -ne $null } | Select-Object -First 1
    
if ([string]::IsNullOrEmpty($Version)) {
    Write-Host "ERROR: Could not read version from PhoneAssistant.WPF.csproj" -ForegroundColor Red
    exit 1
}

if ($build)
{
    Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
    if (Test-Path ".\publish") {
        Remove-Item ".\publish" -Recurse -Force
    }
    New-Item -ItemType Directory -Path ".\publish" | Out-Null
    
    Write-Host "Building version: $Version" -ForegroundColor Green

    dotnet publish PhoneAssistant.WPF/PhoneAssistant.WPF.csproj `
        -c Release `
        -o publish `
        -r win-x64 `
        --self-contained false 

     if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ PhoneAssistant build complete" -ForegroundColor Green
    } else {
        Write-Host "✗ PhoneAssistant build failed" -ForegroundColor Red
        exit 1
    }

    dotnet publish PhoneAssistant.Cli/PhoneAssistant.Cli.csproj `
        -c Release `
        -o publish `
        -r win-x64 `
        --self-contained false 

     if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ pac build complete" -ForegroundColor Green
    } else {
        Write-Host "✗ pac build failed" -ForegroundColor Red
        exit 1
    }

    dotnet publish Reconcile/Reconcile.csproj `
        -c Release `
        -o publish `
        -r win-x64 `
        --self-contained false 

     if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Reconcile build complete" -ForegroundColor Green
    } else {
        Write-Host "✗ Reconcile build failed" -ForegroundColor Red
        exit 1
    }

    # Summary
    Write-Host ""
    Write-Host "Build Complete!" -ForegroundColor Cyan
    Write-Host "==================" -ForegroundColor Cyan
    Get-ChildItem ".\publish\*.exe" | ForEach-Object {
        $size = [math]::Round($_.Length / 1MB, 2)
        Write-Host "$($_.Name) - $size MB" -ForegroundColor White
    }
    Write-Host ""
    Write-Host "Output directory: .\publish\" -ForegroundColor Yellow
}

if ($Pack)
{
    Write-Host "Cleaning previous Releases..." -ForegroundColor Yellow
    if (Test-Path ".\Releases") {
        Remove-Item ".\Releases" -Recurse -Force
    }
    New-Item -ItemType Directory -Path ".\Releases" | Out-Null

    vpk download local --path "\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application"

    vpk pack -u PhoneAssistant `
        -v $Version `
        -p .\publish `
        -e PhoneAssistant.exe `
        -i PhoneAssistant.WPF\Resources\Phone.ico `
        --packAuthors "Devon County Council" `
        --noPortable 

    vpk upload local --keepMaxReleases 6 --path "\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application"
}
