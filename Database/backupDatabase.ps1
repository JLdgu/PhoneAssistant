# Backup database
param (
    [switch]$test
)

$backupDate = Get-Date -Format "yyyy-MM-dd"
$db = "PhoneAssistant.db"
$backupDir = "Backup"
$backupDb = "PhoneAssistant$backupDate.db"

$livePath = "K:/FITProject/ICTS/Mobile Phones/PhoneAssistant"
$testPath =  "c:/temp" 

if ($test){
    $dbPath = Join-Path $testPath $db
    $backupPath = Join-Path $testPath $backupDir
    $backupPath = Join-Path $backupPath $backupDb
}
else {
    $dbPath = Join-Path $livePath $db
    $backupPath = Join-Path $livePath $backupDir
    $backupPath = Join-Path $backupPath $backupDb
}

Write-Host "Backing up $dbPath to $backupPath"
Copy-Item $dbPath $backupPath