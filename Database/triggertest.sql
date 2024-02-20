INSERT INTO Phones (IMEI,PhoneNumber,SIMNumber,FormerUser,NorR,Status,OEM,Model,SRNumber,AssetTag,Notes) 
    VALUES ('PhoneTriggerTest', 'PN', 'SN', 'FU', 'N','S', 'Apple', 'M', 0, 'AT', 'Note');
SELECT LastUpdate FROM Phones WHERE IMEI = 'PhoneTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET AssetTag = 'U_AT' WHERE IMEI = 'PhoneTriggerTest';
SELECT AssetTag, LastUpdate FROM Phones WHERE IMEI = 'PhoneTriggerTest';
SELECT UpdateType, AssetTag, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest' AND AssetTag = 'AT';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET FormerUser = 'U_FU' WHERE IMEI = 'PhoneTriggerTest';
SELECT FormerUser, LastUpdate FROM Phones WHERE IMEI = 'PhoneTriggerTest';
SELECT UpdateType, FormerUser, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest' AND FormerUser = 'FU';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET Model = 'U_M' WHERE IMEI = 'PhoneTriggerTest';
SELECT Model, LastUpdate FROM Phones WHERE IMEI = 'PhoneTriggerTest';
SELECT UpdateType, Model, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest' AND Model = 'M';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET NorR = 'R' WHERE IMEI = 'PhoneTriggerTest';
SELECT NorR, LastUpdate FROM Phones WHERE IMEI = 'PhoneTriggerTest';
SELECT UpdateType, NorR, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest' AND NorR = 'N';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET Notes = 'U_Note' WHERE IMEI = 'PhoneTriggerTest';
SELECT Notes, LastUpdate FROM Phones WHERE IMEI = 'PhoneTriggerTest';
SELECT UpdateType, Notes, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest' AND Notes = 'Note';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET PhoneNumber = 'U_PN' WHERE IMEI = 'PhoneTriggerTest';
SELECT PhoneNumber, LastUpdate FROM Phones WHERE IMEI = 'PhoneTriggerTest';
SELECT UpdateType, PhoneNumber, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest' AND PhoneNumber = 'PN';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET SIMNumber = 'U_SN' WHERE IMEI = 'PhoneTriggerTest';
SELECT SIMNumber, LastUpdate FROM Phones WHERE IMEI = 'PhoneTriggerTest';
SELECT UpdateType, SIMNumber, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest' AND SIMNumber = 'SN';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET SRNumber = 2 WHERE IMEI = 'PhoneTriggerTest';
SELECT SRNumber, LastUpdate FROM Phones WHERE IMEI = 'PhoneTriggerTest';
SELECT UpdateType, SRNumber, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest' AND SRNumber = 0;
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE Phones SET Status = 'U_S' WHERE IMEI = 'PhoneTriggerTest';
SELECT Status, LastUpdate FROM Phones WHERE IMEI = 'PhoneTriggerTest';
SELECT UpdateType, Status, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest' AND Status = 'S';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

DELETE FROM Phones WHERE IMEI = 'PhoneTriggerTest';
SELECT UpdateType, LastUpdate FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';
DELETE FROM UpdateHistoryPhones WHERE IMEI = 'PhoneTriggerTest';

INSERT INTO SIMs (PhoneNumber,SIMNumber,Status,SR,Notes,AssetTag) 
    VALUES ('SIMsTriggerTest', 'SN', 'S', 0, 'N', 'A');
SELECT LastUpdate FROM SIMs WHERE PhoneNumber = 'SIMsTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE SIMs SET AssetTag = 'U_A' WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT Notes, LastUpdate FROM SIMs WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT UpdateType, Notes, LastUpdate FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest' AND AssetTag = 'A';
DELETE FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE SIMs SET Notes = 'U_N' WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT Notes, LastUpdate FROM SIMs WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT UpdateType, Notes, LastUpdate FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest' AND Notes = 'N';
DELETE FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE SIMs SET SIMNumber = 'U_SN' WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT SIMNumber, LastUpdate FROM SIMs WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT UpdateType, SIMNumber, LastUpdate FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest' AND SIMNumber = 'SN';
DELETE FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE SIMs SET SR = 3 WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT SR, LastUpdate FROM SIMs WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT UpdateType, SR, LastUpdate FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest' AND SR = 0;
DELETE FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

UPDATE SIMs SET Status = 'U_S' WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT Status, LastUpdate FROM SIMs WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT UpdateType, Status, LastUpdate FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest' AND Status = 'S';
DELETE FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest';
WITH RECURSIVE r(i) AS (VALUES(0) UNION ALL SELECT i FROM r LIMIT 3000000) SELECT i FROM r WHERE i = 1;

DELETE FROM SIMs WHERE PhoneNumber = 'SIMsTriggerTest';
SELECT UpdateType, LastUpdate FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest';
DELETE FROM UpdateHistorySIMs WHERE PhoneNumber = 'SIMsTriggerTest';

.exit 1