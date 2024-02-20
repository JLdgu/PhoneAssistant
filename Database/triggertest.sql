INSERT INTO Phones (IMEI,PhoneNumber,SIMNumber,FormerUser,NorR,Status,OEM,Model,AssetTag,Notes) 
    VALUES ('PhoneInsertTriggerTest', 'PN', 'SN', 'FU', 'N','S', 'Apple', 'M', 'AT', 'Note');
SELECT LastUpdate FROM Phones WHERE IMEI = 'PhoneInsertTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET AssetTag = 'U_AT' WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT AssetTag, LastUpdate FROM Phones WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT UpdateType, AssetTag, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest' AND AssetTag = 'AT';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET FormerUser = 'U_FU' WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT FormerUser, LastUpdate FROM Phones WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT UpdateType, FormerUser, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest' AND FormerUser = 'FU';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET Model = 'U_M' WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT Model, LastUpdate FROM Phones WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT UpdateType, Model, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest' AND Model = 'M';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET NorR = 'R' WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT NorR, LastUpdate FROM Phones WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT UpdateType, NorR, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest' AND NorR = 'N';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET Notes = 'U_Note' WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT Notes, LastUpdate FROM Phones WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT UpdateType, Notes, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest' AND Notes = 'Note';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET PhoneNumber = 'U_PN' WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT PhoneNumber, LastUpdate FROM Phones WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT UpdateType, PhoneNumber, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest' AND PhoneNumber = 'PN';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET SIMNumber = 'U_SN' WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT SIMNumber, LastUpdate FROM Phones WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT UpdateType, SIMNumber, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest' AND SIMNumber = 'SN';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET Status = 'U_S' WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT Status, LastUpdate FROM Phones WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT UpdateType, Status, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest' AND Status = 'S';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

DELETE FROM Phones WHERE IMEI = 'PhoneInsertTriggerTest';
SELECT UpdateType, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneInsertTriggerTest';

.exit 1
