# Export schema
$db =  "c:/temp/paTest.db" 
Remove-Item "schema.sql"

$env:Path += ';c:\temp\sqlite'
sqlite3 $db .schema > schema.sql
