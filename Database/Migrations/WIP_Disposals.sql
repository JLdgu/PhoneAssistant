-- Move Disposals to Phones
-- Create ReconcileDisposals

DROP TRIGGER Phones_Delete;
DROP TRIGGER Phones_Update;
ALTER TABLE Phones RENAME TO PhonesOld;

CREATE TABLE Phones (
	IMEI	        TEXT NOT NULL PRIMARY KEY,
	PhoneNumber	    TEXT,
	SIMNumber	    TEXT,
	FormerUser	    TEXT,
	NorR	        TEXT NOT NULL CHECK("NorR" = 'N' OR "NorR" = 'R'),
	Status	        TEXT NOT NULL,
	OEM	            TEXT NOT NULL CHECK("OEM" = 'Apple' OR "OEM" = 'Nokia' OR "OEM" = 'Samsung' OR "OEM" = 'Other'),
	Model	        TEXT NOT NULL,
	SRNumber	    INTEGER,
	AssetTag	    TEXT,
	NewUser	        TEXT,
	Notes	        TEXT,
	Collection	    INTEGER,
	DespatchDetails	TEXT,
	LastUpdate	    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);
INSERT INTO Phones (
    "AssetTag","Collection","DespatchDetails","FormerUser","IMEI","LastUpdate","Model","NewUser","NorR","Notes","OEM","PhoneNumber","SIMNumber","SRNumber","Status") 
    SELECT "AssetTag","Collection","DespatchDetails","FormerUser","IMEI","LastUpdate","Model","NewUser","NorR","Notes","OEM","PhoneNumber","SIMNumber","SRNumber","Status" FROM PhonesOld;

DROP TABLE PhonesOld;

CREATE TRIGGER Phones_Delete
    AFTER DELETE ON Phones
    FOR EACH ROW
BEGIN
	INSERT INTO UpdateHistoryPhones (        
        AssetTag, Collection, DespatchDetails, FormerUser, IMEI, LastUpdate, Model, 
        NewUser, NorR, Notes, OEM, PhoneNumber, SIMNumber, SRNumber, Status, UpdateType
        ) 
    VALUES (
        old.AssetTag, old.Collection, old.DespatchDetails, old.FormerUser, old.IMEI, CURRENT_TIMESTAMP, old.Model, 
        old.NewUser, old.NorR, old.Notes, old.OEM, old.PhoneNumber, old.SIMNumber, old.SRNumber, old.Status, 'DELETE'        
    );
END;

CREATE TRIGGER Phones_Update
    AFTER UPDATE ON Phones
    FOR EACH ROW
BEGIN
    UPDATE Phones SET LastUpdate = CURRENT_TIMESTAMP WHERE IMEI = old.IMEI;
	INSERT INTO UpdateHistoryPhones (        
        AssetTag, Collection, DespatchDetails, FormerUser, IMEI, LastUpdate, Model, 
        NewUser, NorR, Notes, OEM, PhoneNumber, SIMNumber, SRNumber, Status, UpdateType
        ) 
    VALUES (
        old.AssetTag, old.Collection, old.DespatchDetails, old.FormerUser, old.IMEI, CURRENT_TIMESTAMP, old.Model, 
        old.NewUser, old.NorR, old.Notes, old.OEM, old.PhoneNumber, old.SIMNumber, old.SRNumber, old.Status, 'UPDATE'
    );
END;

DELETE FROM Disposals
WHERE IMEI is NULL;

INSERT INTO Phones (
    IMEI, FormerUser, NorR, Status, OEM, Model, SRNumber, AssetTag )
   SELECT IMEI, FormerUser, 'R', Status, OEM, 'Unknown', SRNumber, AssetTag
   FROM Disposals
   WHERE "OEM" = 'Apple' OR "OEM" = 'Nokia' OR "OEM" = 'Samsung';

DELETE FROM Disposals
WHERE "OEM" = 'Apple' OR "OEM" = 'Nokia' OR "OEM" = 'Samsung';

INSERT INTO Phones (
    IMEI, FormerUser, NorR, Status, OEM, Model, SRNumber, AssetTag )
   SELECT IMEI, FormerUser, 'R', Status , 'Other' , 'Unknown', SRNumber, AssetTag
   FROM Disposals;
   
DROP TABLE Disposals;

CREATE TABLE ReconcileDisposals (
	IMEI	        TEXT NOT NULL PRIMARY KEY,
	StatusMS       TEXT,	
	StatusPA        TEXT,
    StatusSCC       TEXT,
    SR              INTEGER,
    Certificate     INTEGER,
    Action          TEXT
);
.exit 1