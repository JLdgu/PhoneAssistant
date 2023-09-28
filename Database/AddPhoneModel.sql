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
    Model           TEXT, 
	SRNumber    	INTEGER,
	AssetTag	    TEXT,
	NewUser         TEXT,
	Notes	        TEXT,
	LastUpdate	    TEXT DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY("IMEI")
);

INSERT INTO Phones (
    AssetTag, FormerUser,IMEI,LastUpdate,NewUser,NorR,Notes,OEM,PhoneNumber,SIMNumber,SRNumber,Status
) 
    SELECT AssetTag, FormerUser, IMEI, LastUpdate, NewUser, NorR, Notes, OEM, PhoneNumber, SIMNumber, SRNumber, Status
    FROM OldPhones;

DROP TABLE OldPhones;

UPDATE Phones
    SET Model = 'iPhone'
    WHERE NorR = 'R' AND OEM = 'Apple' AND Model IS NULL;

UPDATE Phones
    SET Model = '105'
    WHERE OEM = 'Nokia' AND Model IS NULL;

UPDATE Phones
    SET Model = 'A32'
    WHERE NorR = 'R' AND OEM = 'Samsung' AND Model IS NULL;

UPDATE Phones
    SET Model = 'A33'
    WHERE NorR = 'N' AND OEM = 'Samsung' AND Model IS NULL;

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
	JobTitle    	TEXT,
	Department	    TEXT,
	LastUpdate      TEXT NOT NULL,
    PRIMARY KEY("Id" AUTOINCREMENT)
);

INSERT INTO UpdateHistoryPhones (
    Id, AssetTag, FormerUser,IMEI,LastUpdate,NewUser,NorR,Notes,OEM,PhoneNumber,SIMNumber,SRNumber,Status,UpdateType
) 
    SELECT Id, AssetTag, FormerUser, IMEI, LastUpdate, NewUser, NorR, Notes, OEM, PhoneNumber, SIMNumber, SRNumber, Status, UpdateType
    FROM OldUpdateHistoryPhones;

DROP TABLE OldUpdateHistoryPhones;

CREATE TRIGGER Phones_Delete
    AFTER DELETE ON Phones
    FOR EACH ROW
BEGIN
	INSERT INTO UpdateHistoryPhones (        
        AssetTag, FormerUser, IMEI, LastUpdate, Model, NewUser, NorR, 
        Notes, OEM, PhoneNumber, SIMNumber, SRNumber, Status, UpdateType
        ) 
    VALUES (
        old.AssetTag, old.FormerUser, old.IMEI, old.LastUpdate, old.Model, old.NewUser, old.NorR, 
        old.Notes, old.OEM, old.PhoneNumber, old.SIMNumber, old.SRNumber, old.Status, 'DELETE'        
    );
END;

CREATE TRIGGER Phones_Update
    AFTER UPDATE ON Phones
    FOR EACH ROW
    WHEN NEW.LastUpdate = OLD.LastUpdate    
BEGIN
    UPDATE Phones SET LastUpdate = CURRENT_TIMESTAMP WHERE IMEI = old.IMEI;
	INSERT INTO UpdateHistoryPhones (        
        AssetTag, FormerUser, IMEI, LastUpdate, Model, NewUser, NorR, 
        Notes, OEM, PhoneNumber, SIMNumber, SRNumber, Status, UpdateType
        ) 
    VALUES (
        old.AssetTag, old.FormerUser, old.IMEI, old.LastUpdate, old.Model, old.NewUser, old.NorR, 
        old.Notes, old.OEM, old.PhoneNumber, old.SIMNumber, old.SRNumber, old.Status, 'UPDATE'
    );
END;

.EXIT 1