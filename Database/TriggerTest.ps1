$scriptPath = (Get-Item .).FullName
$script = Join-Path $scriptPath "triggertest.sql"
If (!(Test-Path -Path $script))
{   
    Write-Host "$script not found"
    Exit
}

./live2test.ps1

$dbPath =  "c:/temp/paTest.db"  
$env:Path += ';c:\temp\sqlite'
Write-Host "Database $dbPath" 
sqlite3 $dbPath -init $script