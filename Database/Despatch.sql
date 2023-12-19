DROP TRIGGER Phones_Delete;
DROP TRIGGER Phones_Update;
ALTER TABLE Phones RENAME TO OldPhones;
CREATE TABLE Phones (
	IMEI	        TEXT NOT NULL,
	PhoneNumber	    TEXT,
	SIMNumber	    TEXT,
	FormerUser	    TEXT,
	NorR	        TEXT NOT NULL CHECK("NorR" = 'N' OR "NorR" = 'R'),
	Status	        TEXT NOT NULL,
	OEM	            TEXT NOT NULL CHECK("OEM" = 'Apple' OR "OEM" = 'Nokia' OR "OEM" = 'Samsung'),
	Model	        TEXT NOT NULL,
	SRNumber	    INTEGER,
	AssetTag	    TEXT,
	NewUser	        TEXT,
	Notes	        TEXT,
	Collection	    INTEGER,
	DespatchDetails	TEXT,
	LastUpdate	    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY("IMEI")
);
INSERT INTO Phones ("AssetTag","FormerUser","IMEI","LastUpdate","Model","NewUser","NorR","Notes","OEM","PhoneNumber","SIMNumber","SRNumber","Status") 
    SELECT "AssetTag","FormerUser","IMEI","LastUpdate","Model","NewUser","NorR","Notes","OEM","PhoneNumber","SIMNumber","SRNumber","Status" FROM OldPhones;
DROP TABLE OldPhones;

ALTER TABLE UpdateHistoryPhones RENAME TO OldUpdateHistoryPhones;
CREATE TABLE UpdateHistoryPhones (
    Id INTEGER      NOT NULL,
    UpdateType      TEXT NOT NULL,
    IMEI            TEXT NOT NULL,
    PhoneNumber     TEXT,
    SIMNumber       INTEGER,
    FormerUser      TEXT,
    NorR            TEXT,
    Status          TEXT,
    OEM             TEXT,
    Model           TEXT,
    SRNumber        INTEGER,
    AssetTag        TEXT,
    NewUser         TEXT,
    Notes           TEXT,
	Collection    	INTEGER,
	DespatchDetails TEXT,
	LastUpdate      TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY("Id" AUTOINCREMENT)
);
INSERT INTO UpdateHistoryPhones ("AssetTag","FormerUser","IMEI","Id","LastUpdate","Model","NewUser","NorR","Notes","OEM","PhoneNumber","SIMNumber","SRNumber","Status","UpdateType") 
    SELECT "AssetTag","FormerUser","IMEI","Id","LastUpdate","Model","NewUser","NorR","Notes","OEM","PhoneNumber","SIMNumber","SRNumber","Status","UpdateType" FROM OldUpdateHistoryPhones;
DROP TABLE OldUpdateHistoryPhones;

CREATE TRIGGER Phones_Delete
    AFTER DELETE ON Phones
    FOR EACH ROW
BEGIN
	INSERT INTO UpdateHistoryPhones (        
        AssetTag, Collection, DespatchDetails, FormerUser, IMEI, Model, 
        NewUser, NorR, Notes, OEM, PhoneNumber, SIMNumber, SRNumber, Status, UpdateType
        ) 
    VALUES (
        old.AssetTag, old.Collection, old.DespatchDetails, old.FormerUser, old.IMEI, old.Model, 
        old.NewUser, old.NorR, old.Notes, old.OEM, old.PhoneNumber, old.SIMNumber, old.SRNumber, old.Status, 'DELETE'        
    );
END;
CREATE TRIGGER Phones_Update
    AFTER UPDATE ON Phones
    FOR EACH ROW
    WHEN NEW.LastUpdate = OLD.LastUpdate    
BEGIN
    UPDATE Phones SET LastUpdate = CURRENT_TIMESTAMP WHERE IMEI = old.IMEI;
	INSERT INTO UpdateHistoryPhones (        
        AssetTag, Collection, DespatchDetails, FormerUser, IMEI, Model, 
        NewUser, NorR, Notes, OEM, PhoneNumber, SIMNumber, SRNumber, Status, UpdateType
        ) 
    VALUES (
        old.AssetTag, old.Collection, old.DespatchDetails, old.FormerUser, old.IMEI, old.Model, 
        old.NewUser, old.NorR, old.Notes, old.OEM, old.PhoneNumber, old.SIMNumber, old.SRNumber, old.Status, 'UPDATE'
    );
END;
.exit 1