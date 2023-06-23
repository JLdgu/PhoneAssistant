$dbFile = "c:/temp/disposals.db"
if (Test-Path $dbFile) { Remove-Item $dbFile -verbose }
c:/temp/SQLite/sqlite3.exe -init .\disposals.sqlite3