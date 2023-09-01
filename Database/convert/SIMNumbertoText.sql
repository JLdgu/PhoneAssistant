CREATE TABLE sqlb_temp_table_1 (
	"IMEI"	TEXT NOT NULL,
	"Phone Number"	TEXT,
	"SIM Number"	TEXT,
	"Former User"	TEXT,
	"Wiped"	TEXT,
	"Status"	TEXT,
	"OEM"	TEXT,
	"SR Number"	INTEGER,
	"Asset Tag"	TEXT,
	"New user"	TEXT,
	"Notes"	TEXT,
	"LastUpdate"	TEXT DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY("IMEI")
);
INSERT INTO sqlb_temp_table_1 ("Asset Tag","Former User","IMEI","LastUpdate","New user","Notes","OEM","Phone Number","SIM Number","SR Number","Status","Wiped") 
    SELECT "Asset Tag","Former User","IMEI","LastUpdate","New user","Notes","OEM","Phone Number","SIM Number","SR Number","Status","Wiped" 
    FROM Phones;

DROP TABLE Phones;
ALTER TABLE sqlb_temp_table_1 RENAME TO Phones;

CREATE TRIGGER Phones_Delete
    AFTER DELETE ON Phones
    FOR EACH ROW
BEGIN
	INSERT INTO UpdateHistoryPhones (UpdateType, IMEI, "Phone Number", "SIM Number", "Former User", Wiped, Status, OEM, "SR Number", "Asset Tag", "New User", Notes, LastUpdate) 
    VALUES ('DELETE', old.IMEI, old."Phone Number", old."SIM Number", old."Former User", old.Wiped, old.Status, old.OEM, old."SR Number", old."Asset Tag", old."New User", old.Notes, old.LastUpdate);
END;
CREATE TRIGGER Phones_Update
    AFTER UPDATE ON Phones
    FOR EACH ROW
    WHEN NEW.LastUpdate = OLD.LastUpdate    
BEGIN
   UPDATE Phones SET LastUpdate = CURRENT_TIMESTAMP WHERE IMEI = old.IMEI;
	INSERT INTO UpdateHistoryPhones (UpdateType, IMEI, "Phone Number", "SIM Number", "Former User", Wiped, Status, OEM, "SR Number", "Asset Tag", "New User", Notes, LastUpdate) 
    VALUES ('UPDATE', old.IMEI, old."Phone Number", old."SIM Number", old."Former User", old.Wiped, old.Status, old.OEM, old."SR Number", old."Asset Tag", old."New User", old.Notes, old.LastUpdate);
END;
