
$script = "Migrations/M20240117_Disposals.sql"

$dbPath =  "c:/temp/paTest.db"  
#$dbPath = "p:/ICTS/Mobile Phones/PhoneAssistant/PhoneAssistant.db" 

$env:Path += ';c:\temp\sqlite'
Write-Host "Database $dbPath"
sqlite3 $dbPath -init $script