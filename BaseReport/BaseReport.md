# How to import Devon Base Report into PhoneAssistant.db

## Setup environment
Ensure sqlite3.exe exists in c:/temp/SQLite
Open "Devon Base Report ddmmyy.xlsx"
Save Report sheet as c:/temp/DevonBaseReport.csv

# Make temporary copy of database 
Copy live PhoneAssistant.db to c:/temp/

## Export "reuse" worksheet from "Phones reuse and disposal.xslm"
1. Copy SR197395 worksheet to new worksheet
2. Delete any columns to right of table
3. Select Table and Convert to Range
4. Delete Row 1 (Column Headers)
5. File > Save As SR197395.csv - CSV UTF-8 (Comma delimited) *.csv
6. Repeat for SR199260, SR201899 and SR204153 worksheets

## Export "reuse" worksheet from "Phones reuse and disposal.xslm"
1. Copy Reuse worksheet to new worksheet
2. Delete columns to right of table
3. Select Table and Convert to Range
4. Delete Row 1 (Column Headers)
5. File > Save As Phones.csv - CSV UTF-8 (Comma delimited) *.csv

## Export "Live Sims" worksheet from "Phones reuse and disposal.xslm"
1. Copy Live Sims worksheet to new worksheet
2. Delete columns to right of table
3. Select Table and Convert to Range
4. Delete Row 1 (Column Headers)
5. File > Save As SIMs.csv - CSV UTF-8 (Comma delimited) *.csv

## Import csv files into new database
1. Ensure sqlite3.exe exists in c:/temp/SQLite
2. Open PowerShell session in PhoneAssistant/convert folder
3. Run PowerShell script convert.ps1