
$script = "Migrations/Migrationyymmdd_name.sql"

$dbPath =  "c:/temp/paTest.db"  
#$dbPath = "p:/ICTS/Mobile Phones/PhoneAssistant/PhoneAssistant.db" 

$env:Path += ';c:\temp\sqlite'
Write-Host "Database $dbPath"
sqlite3 $dbPath -init $script