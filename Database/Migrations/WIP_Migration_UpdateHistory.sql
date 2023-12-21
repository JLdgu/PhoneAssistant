DROP TRIGGER Phones_Delete;
DROP TRIGGER Phones_Update;
DROP TABLE UpdateHistoryPhones;

CREATE TABLE UpdateHistoryPhones (
    Id                  INTEGER      NOT NULL,
    IMEIOld             TEXT NOT NULL,
    IMEINew             TEXT NOT NULL,
    PhoneNumberOld      TEXT,
    PhoneNumberNew      TEXT,
    SIMNumberOld        TEXT, 
    SIMNumberNew        TEXT, 
    FormerUser      TEXT,
    NorR            TEXT,
    StatusOld           TEXT, 
    StatusNew           TEXT, 
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

DROP TRIGGER SIMs_Delete;
DROP TRIGGER SIMs_Update;
DROP TABLE UpdateHistorySIMs;

ALTER TABLE SIMs RENAME TO SIMsOld;
CREATE TABLE SIMs 
(
	PhoneNumber	    TEXT NOT NULL,
	SIMNumber	    TEXT NOT NULL,
	Notes	        TEXT,
	LastUpdate	    TEXT DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY("PhoneNumber")
);
INSERT INTO SIMs (LastUpdate, Notes, PhoneNumber, SIMNumber) 
    SELECT LastUpdate, Notes, PhoneNumber, SIMNumber
    FROM SIMsOld;
DROP TABLE SIMsOld;

CREATE TABLE UpdateHistorySIMs
(
	Id                  INTEGER NOT NULL,
    PhoneNumberOld      TEXT NOT NULL,
    PhoneNumberNew      TEXT NOT NULL,
    SIMNumberOld        TEXT NOT NULL, 
    SIMNumberNew        TEXT NOT NULL, 
    NotesOld            TEXT, 
    NotesNew            TEXT, 
	LastUpdate          TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
.exit 1