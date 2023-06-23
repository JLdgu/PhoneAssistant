$dbFile = "c:/temp/phoneassistant.db"
if (Test-Path $dbFile) { Remove-Item $dbFile -verbose }
c:/temp/SQLite/sqlite3.exe -init .\convert.sqlite3