ALTER TABLE BaseReport RENAME COLUMN [Connected IMEI] TO ConnectedIMEI;
ALTER TABLE BaseReport RENAME COLUMN [Contract End Date] TO ContractEndDate;
ALTER TABLE BaseReport RENAME COLUMN [Current Talkplan Name] TO TalkPlan;
ALTER TABLE BaseReport RENAME COLUMN [Handset Manufacturer] TO Handset;
ALTER TABLE BaseReport RENAME COLUMN [Last Used IMEI] TO LastUsedIMEI;
ALTER TABLE BaseReport RENAME COLUMN [Sim Number] TO SIMNumber;
ALTER TABLE BaseReport RENAME COLUMN "Telephone Number" TO PhoneNumber;
ALTER TABLE BaseReport RENAME COLUMN "User Name" TO UserName;

DROP TRIGGER Phones_Delete;
DROP TRIGGER Phones_Update;

ALTER TABLE Phones RENAME TO OldPhones;

CREATE TABLE Phones (
	IMEI	        TEXT NOT NULL,
	PhoneNumber	    TEXT,
	SIMNumber	    TEXT,
	FormerUser	    TEXT,
	NorR	        TEXT CHECK (NorR = 'N' OR NorR = 'R'),
	Status	        TEXT,
	OEM	            TEXT CHECK (OEM = 'Apple' OR OEM = 'Nokia' OR OEM = 'Samsung'),
	SRNumber    	INTEGER,
	AssetTag	    TEXT,
	NewUser         TEXT,
	Notes	        TEXT,
	JobTitle    	TEXT,
	Department	    TEXT,
	LastUpdate	    TEXT DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY("IMEI")
);

INSERT INTO Phones (
    AssetTag, FormerUser,IMEI,LastUpdate,NewUser,Notes,OEM,PhoneNumber,SIMNumber,SRNumber,Status,NorR
) 
    SELECT "Asset Tag","Former User",IMEI,"LastUpdate","New user","Notes",OEM,"Phone Number","SIM Number","SR Number",Status, 'R' 
    FROM OldPhones;

DROP TABLE OldPhones;

DROP TRIGGER SIMs_Delete;
DROP TRIGGER SIMs_Update;

ALTER TABLE SIMs RENAME COLUMN [Asset Tag] TO AssetTag;
ALTER TABLE SIMs RENAME COLUMN [Phone Number] TO PhoneNumber;
ALTER TABLE SIMs RENAME COLUMN [Sim Number] TO SIMNumber;

ALTER TABLE SIMs DROP COLUMN [Call Forwarding];
ALTER TABLE SIMs DROP COLUMN [Voice Mail];

ALTER TABLE UpdateHistorySIMs RENAME COLUMN [Asset Tag] TO AssetTag;
ALTER TABLE UpdateHistorySIMs RENAME COLUMN [Phone Number] TO PhoneNumber;
ALTER TABLE UpdateHistorySIMs RENAME COLUMN [SIM Number] TO SIMNumber;
ALTER TABLE UpdateHistorySIMs DROP COLUMN [Call Forwarding];
ALTER TABLE UpdateHistorySIMs DROP COLUMN [Voice Mail];

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
    SRNumber        INTEGER,
    AssetTag        TEXT,
    NewUser         TEXT,
    Notes           TEXT,
	JobTitle    	TEXT,
	Department	    TEXT,
	LastUpdate      TEXT NOT NULL,
    PRIMARY KEY("Id" AUTOINCREMENT)
);

INSERT INTO UpdateHistoryPhones (
    Id, AssetTag, FormerUser,IMEI,LastUpdate,NewUser,Notes,OEM,PhoneNumber,SIMNumber,SRNumber,Status,UpdateType,NorR
) 
    SELECT Id, "Asset Tag","Former User",IMEI,LastUpdate,"New user",Notes,OEM,"Phone Number","SIM Number","SR Number",Status,UpdateType,'R' 
    FROM OldUpdateHistoryPhones;

DROP TABLE OldUpdateHistoryPhones;

CREATE TRIGGER Phones_Delete
    AFTER DELETE ON Phones
    FOR EACH ROW
BEGIN
	INSERT INTO UpdateHistoryPhones (        
        AssetTag, Department, FormerUser, IMEI, Jobtitle, LastUpdate, NewUser, NorR, Notes, OEM, PhoneNumber, SIMNumber, SRNumber, Status, UpdateType
        ) 
    VALUES (
        old.AssetTag, old.Department, old.FormerUser, old.IMEI, old.JobTitle, old.LastUpdate, old.NewUser, 
        old.NorR, old.Notes, old.OEM, old.PhoneNumber, old.SIMNumber, old.SRNumber, old.Status, 'DELETE'        
    );
END;

CREATE TRIGGER Phones_Update
    AFTER UPDATE ON Phones
    FOR EACH ROW
    WHEN NEW.LastUpdate = OLD.LastUpdate    
BEGIN
    UPDATE Phones SET LastUpdate = CURRENT_TIMESTAMP WHERE IMEI = old.IMEI;
	INSERT INTO UpdateHistoryPhones (        
        AssetTag, Department, FormerUser, IMEI, Jobtitle, LastUpdate, NewUser, NorR, Notes, OEM, PhoneNumber, SIMNumber, SRNumber, Status, UpdateType
        ) 
    VALUES (
        old.AssetTag, old.Department, old.FormerUser, old.IMEI, old.JobTitle, old.LastUpdate, old.NewUser, 
        old.NorR, old.Notes, old.OEM, old.PhoneNumber, old.SIMNumber, old.SRNumber, old.Status, 'UPDATE'
    );
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

.EXIT 1