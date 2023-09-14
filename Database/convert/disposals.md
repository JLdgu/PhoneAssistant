# Step by Step Instructions for Conversion from Excel to SQLite

## Export "reuse" worksheet from "Phones reuse and disposal.xslm"
1. Copy SR197395 worksheet to new worksheet
2. Delete any columns to right of table
3. Select Table and Convert to Range
4. Delete Row 1 (Column Headers)
5. File > Save As SR197395.csv - CSV UTF-8 (Comma delimited) *.csv
6. Repeat for SR199260, SR201899 and SR204153 worksheets

## Import csv files into new database
1. Ensure sqlite3.exe exists in c:/temp/SQLite
2. Open PowerShell session in PhoneAssistant/convert folder
3. Run PowerShell script disposal.ps1