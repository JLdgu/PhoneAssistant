.open c:/temp/phoneassistant.db
.mode csv
.import c:/temp/DevonBaseReport.csv temp_table
CREATE TABLE BaseReport ([Telephone Number], [User Name], [Contract End Date], [Current Talkplan Name], [Handset Manufacturer], [Sim Number], [Connected IMEI], [Last Used IMEI]); 
INSERT INTO BaseReport ([Telephone Number], [User Name], [Contract End Date], [Current Talkplan Name], [Handset Manufacturer], [Sim Number], [Connected IMEI], [Last Used IMEI])
SELECT [Telephone Number], [User Name], [Contract End Date], [Current Talkplan Name], [Handset Manufacturer], [Sim Number], [Connected IMEI], [Last Used IMEI]
FROM temp_table;
DROP TABLE temp_table;