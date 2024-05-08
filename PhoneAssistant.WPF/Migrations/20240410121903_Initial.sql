CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

DROP TRIGGER Phones_Update;

ALTER TABLE Phones DROP COLUMN Collection;

CREATE TABLE "sqlb_temp_table_1" (
	"Id"	INTEGER NOT NULL,
	"UpdateType"	TEXT NOT NULL,
	"IMEI"	TEXT NOT NULL,
	"PhoneNumber"	TEXT,
	"SIMNumber"	TEXT,
	"FormerUser"	TEXT,
	"NorR"	BLOB,
	"Status"	TEXT,
	"OEM"	TEXT,
	"Model"	TEXT,
	"SRNumber"	INTEGER,
	"AssetTag"	TEXT,
	"NewUser"	TEXT,
	"Notes"	TEXT,
	"DespatchDetails"	TEXT,
	"LastUpdate"	TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY("Id" AUTOINCREMENT)
);

INSERT INTO "main"."sqlb_temp_table_1" ("AssetTag","DespatchDetails","FormerUser","IMEI","Id","LastUpdate","Model","NewUser","NorR","Notes","OEM","PhoneNumber","SIMNumber","SRNumber","Status","UpdateType") 
	SELECT "AssetTag","DespatchDetails","FormerUser","IMEI","Id","LastUpdate","Model","NewUser","NorR","Notes","OEM","PhoneNumber","SIMNumber","SRNumber","Status","UpdateType" FROM "main"."UpdateHistoryPhones";

DROP TABLE "main"."UpdateHistoryPhones";

ALTER TABLE "main"."sqlb_temp_table_1" RENAME TO "UpdateHistoryPhones";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240410121903_Initial', '8.0.4');
