CREATE TABLE "sqlb_temp_table_1" (
	"IMEI"	TEXT NOT NULL,
	"PhoneNumber"	TEXT,
	"SIMNumber"	TEXT,
	"FormerUser"	TEXT,
	"NorR"	TEXT NOT NULL CHECK("NorR" = 'N' OR "NorR" = 'R'),
	"Status"	TEXT NOT NULL,
	"OEM"	TEXT NOT NULL CHECK("OEM" = 'Apple' OR "OEM" = 'Nokia' OR "OEM" = 'Samsung'),
	"Model"	TEXT NOT NULL,
	"SRNumber"	INTEGER,
	"AssetTag"	TEXT,
	"NewUser"	TEXT,
	"Notes"	TEXT,
	"LastUpdate"	TEXT DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY("IMEI")
);
INSERT INTO "main"."sqlb_temp_table_1" ("AssetTag","FormerUser","IMEI","LastUpdate","Model","NewUser","NorR","Notes","OEM","PhoneNumber","SIMNumber","SRNumber","Status") SELECT "AssetTag","FormerUser","IMEI","LastUpdate","Model","NewUser","NorR","Notes","OEM","PhoneNumber","SIMNumber","SRNumber","Status" FROM "main"."Phones";
DROP TABLE "main"."Phones";
ALTER TABLE "main"."sqlb_temp_table_1" RENAME TO "Phones";
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