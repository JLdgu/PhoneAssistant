CREATE TABLE SIMs
(
    "PhoneNumber" TEXT NOT NULL,
    "SIMNumber" TEXT NOT NULL, 
    Status TEXT, 
    SR INTEGER, 
    Notes TEXT, 
    "AssetTag" TEXT,
	LastUpdate TEXT DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY("PhoneNumber")
 );
CREATE TABLE sqlite_sequence(name,seq);
CREATE TABLE UpdateHistorySIMs

(

	"Id" INTEGER NOT NULL,

	UpdateType TEXT NOT NULL,

    "PhoneNumber" TEXT NOT NULL,

    "SIMNumber" TEXT NOT NULL, 

    Status TEXT, 

    SR INTEGER, 

    Notes TEXT, 

    "AssetTag" TEXT,

	LastUpdate TEXT NOT NULL,

	PRIMARY KEY("Id" AUTOINCREMENT)

);
CREATE TABLE BaseReport ("PhoneNumber", "UserName", "ContractEndDate", "TalkPlan", "Handset", "SIMNumber", "ConnectedIMEI", "LastUsedIMEI");
CREATE TABLE IF NOT EXISTS "Disposals" (

	"IMEI"	TEXT UNIQUE,

	"FormerUser"	TEXT,

	"OEM"	TEXT,

	"Status"	TEXT,

	"SRNumber"	INTEGER,

	"Certificate"	INTEGER,

	"AssetTag"	TEXT,

	"SerialNumber"	TEXT,

	"LastUpdate"	TEXT DEFAULT CURRENT_TIMESTAMP,

	PRIMARY KEY("IMEI")

);
CREATE TRIGGER Disposals_Update

    AFTER UPDATE ON Disposals

    FOR EACH ROW

    WHEN NEW.LastUpdate = OLD.LastUpdate    

BEGIN

   UPDATE Disposals SET LastUpdate = CURRENT_TIMESTAMP WHERE IMEI = old.IMEI;

END;
CREATE TRIGGER SIMs_Delete
    AFTER DELETE ON SIMs
    FOR EACH ROW
BEGIN
	INSERT INTO UpdateHistorySIMs (UpdateType, PhoneNumber, SIMNumber, Status, SR, Notes, AssetTag, LastUpdate) 
    VALUES ('DELETE', old.PhoneNumber, old.SIMNumber, old.Status, old.SR, old.Notes, old.AssetTag, old.LastUpdate);
END;
CREATE TRIGGER SIMs_Update
    AFTER UPDATE ON SIMs
    FOR EACH ROW
    WHEN NEW.LastUpdate = OLD.LastUpdate    
BEGIN
    UPDATE SIMs SET LastUpdate = CURRENT_TIMESTAMP WHERE PhoneNumber = old.PhoneNumber;
	INSERT INTO UpdateHistorySIMs (UpdateType, PhoneNumber, SIMNumber, Status, SR, Notes, AssetTag, LastUpdate) 
    VALUES ('UPDATE', old.PhoneNumber, old.SIMNumber, old.Status, old.SR, old.Notes, old.AssetTag, old.LastUpdate);
END;
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
