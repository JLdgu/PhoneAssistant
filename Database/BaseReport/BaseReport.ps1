$sqlite = "c:/temp/sqlite/sqlite3.exe"
if (!(Test-Path $sqlite)) {
     Write-Host "Unable to find $sqlite" -ForegroundColor red -BackgroundColor white
     Exit
}
$base = "c:/temp/DevonBaseReport.csv"
if (!(Test-Path $base)) {
    Write-Host "Unable to find $base" -ForegroundColor red -BackgroundColor white
    Exit
}

$padb_live = "d:/Transfer/.db/phoneassistant.db"
if (!(Test-Path $padb_live)) {
    Write-Host "Unable to find $padb_live" -ForegroundColor red -BackgroundColor white
    Exit
}
$padb_temp = "c:/temp/phoneassistant.db"
if (Test-Path $padb_temp) { 
    Remove-Item $padb_temp
}
Copy-Item $padb_live $padb_temp

Write-Host "Type .quit to exit SQLite" -ForegroundColor green
c:/temp/SQLite/sqlite3.exe -init .\BaseReport.sql

