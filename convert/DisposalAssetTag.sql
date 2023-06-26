CREATE TABLE "sqlb_temp_table_1" (
	"IMEI"	TEXT UNIQUE,
	"Former User"	TEXT,
	"OEM"	TEXT,
	"Status"	TEXT,
	"SR Number"	INTEGER,
	"Certificate"	INTEGER,
	"AssetTag"	TEXT,
	"LastUpdate"	TEXT DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY("IMEI")
);
INSERT INTO "main"."sqlb_temp_table_1" ("Certificate","Former User","IMEI","LastUpdate","OEM","SR Number","Status") 
    SELECT "Certificate","Former User","IMEI","LastUpdate","OEM","SR Number","Status" FROM "main"."Disposals";

DROP TABLE "main"."Disposals";
ALTER TABLE "main"."sqlb_temp_table_1" RENAME TO "Disposals";

CREATE TRIGGER Disposals_Update
    AFTER UPDATE ON Disposals
    FOR EACH ROW
    WHEN NEW.LastUpdate = OLD.LastUpdate    
BEGIN
   UPDATE Disposals SET LastUpdate = CURRENT_TIMESTAMP WHERE IMEI = old.IMEI;
END;