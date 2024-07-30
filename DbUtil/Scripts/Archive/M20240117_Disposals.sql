DROP TRIGGER Disposals_Update;
ALTER TABLE Disposals RENAME TO DisposalsOld;

CREATE TABLE Disposals (
    ROWID           INTEGER PRIMARY KEY,
	IMEI	        TEXT UNIQUE,
	FormerUser	    TEXT,
	OEM	            TEXT,
	Status	        TEXT NOT NULL,
	SRNumber        INTEGER NOT NULL,
	Certificate	    INTEGER,
	AssetTag	    TEXT,
	SerialNumber	TEXT,
	Notes	        TEXT,
	LastUpdate	TEXT DEFAULT CURRENT_TIMESTAMP	
);

INSERT INTO Disposals (AssetTag,Certificate,FormerUser,IMEI,LastUpdate,OEM,SRNumber,SerialNumber,Status) 
    SELECT AssetTag,Certificate,FormerUser,IMEI,LastUpdate,OEM,SRNumber,SerialNumber,Status FROM DisposalsOld;
DROP TABLE DisposalsOld;

CREATE TRIGGER Disposals_Update
    AFTER UPDATE ON Disposals
    FOR EACH ROW
    WHEN NEW.LastUpdate = OLD.LastUpdate    
BEGIN
   UPDATE Disposals SET LastUpdate = CURRENT_TIMESTAMP WHERE IMEI = old.IMEI;
END;
.exit 1