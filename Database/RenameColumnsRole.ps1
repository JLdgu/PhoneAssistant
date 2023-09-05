param (
    [string]$db = "test"
)

$script = "RenameColumnsRole.sql"

switch ($db.ToLower()) {
    "test" { $dbPath =  "c:/temp/paTest.db"  }
    Default { $dbPath = "p:/ICTS/Mobile Phones/PhoneAssistant/PhoneAssistant.db" }    }

$env:Path += ';c:\temp\sqlite'
Write-Host "Database $dbPath"
sqlite3 $dbPath -init $script