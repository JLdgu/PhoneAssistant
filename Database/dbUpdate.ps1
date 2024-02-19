param
(
   [Parameter(Position=0,mandatory=$true)]
   [string] $name,
   [Parameter(Position=1)]
   [bool] $test = $true
)

$scriptPath = (Get-Item .).FullName

$script = Join-Path $scriptPath "Migrations" 
$script = Join-Path $script "$name.sql"

If (!(Test-Path -Path $script))
{   
    Write-Host "$script not found"
    Exit
}

if ($test)
{
    $dbPath =  "c:/temp/paTest.db"  
}
else
{
    Write-Host "LIVE run" 
    #$dbPath = "p:/ICTS/Mobile Phones/PhoneAssistant/PhoneAssistant.db" 
}

$env:Path += ';c:\temp\sqlite'
Write-Host "Database $dbPath" 
sqlite3 $dbPath -init $script