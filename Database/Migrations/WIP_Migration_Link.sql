DROP TRIGGER Phones_Delete;
DROP TRIGGER Phones_Update;
DROP TABLE UpdateHistoryPhones;

ALTER TABLE Phones RENAME TO PhonesOld;

DROP TRIGGER SIMs_Delete;
DROP TRIGGER SIMs_Update;
DROP TABLE UpdateHistorySIMs;

ALTER TABLE SIMs RENAME TO SIMsOld;

CREATE TABLE "Phones" (
    "Imei"          TEXT NOT NULL CONSTRAINT "PK_Phones" PRIMARY KEY,
    "FormerUser"    TEXT NULL,
    "NorR"          TEXT NOT NULL CONSTRAINT "CK_NorR" CHECK ("NorR" = 'N' OR "NorR" = 'R'),
    "Status"        TEXT NOT NULL,
    "OEM"           TEXT NOT NULL CONSTRAINT "CK_OEM" CHECK ("OEM" = 'Apple' OR "OEM" = 'Nokia' OR "OEM" = 'Samsung'),
    "Model"         TEXT NOT NULL,
    "SRNumber"      INTEGER NULL,
    "AssetTag"      TEXT NULL,
    "Notes"         TEXT NULL,
    "LastUpdate"    TEXT NOT NULL    
);

CREATE TABLE "ServiceRequests" (
    "ServiceRequestNumber"  INTEGER NOT NULL CONSTRAINT "PK_ServiceRequests" PRIMARY KEY AUTOINCREMENT,
    "NewUser"               TEXT NOT NULL,
    "Collection"            INTEGER NULL,
    "DespatchDetails"       TEXT NULL,
    "LastUpdate"            TEXT NOT NULL
);

CREATE TABLE "Sims" (
    "PhoneNumber"   TEXT NOT NULL CONSTRAINT "PK_Sims" PRIMARY KEY,
    "SimNumber"     TEXT NOT NULL,
    "Status"        TEXT NULL,
    "AssetTag"      TEXT NULL,
    "Notes"         TEXT NULL,
    "LastUpdate"    TEXT NOT NULL
);

CREATE TABLE "Links" (
    "Id"            INTEGER NOT NULL CONSTRAINT "PK_Links" PRIMARY KEY AUTOINCREMENT,
    "Imei"          TEXT NULL,
    "PhoneNumber"   TEXT NULL,
    "SRNumber"      INTEGER NULL,
    CONSTRAINT "FK_Links_Phones_Imei" FOREIGN KEY ("Imei") REFERENCES "Phones" ("Imei"),
    CONSTRAINT "FK_Links_ServiceRequests_SRNumber" FOREIGN KEY ("SRNumber") REFERENCES "ServiceRequests" ("SRNumber"),
    CONSTRAINT "FK_Links_Sims_PhoneNumber" FOREIGN KEY ("PhoneNumber") REFERENCES "Sims" ("PhoneNumber")
);

CREATE INDEX "IX_Links_Imei" ON "Links" ("Imei");
CREATE INDEX "IX_Links_PhoneNumber" ON "Links" ("PhoneNumber");
CREATE INDEX "IX_Links_SRNumber" ON "Links" ("SRNumber");
CREATE UNIQUE INDEX "IX_Phones_AssetTag" ON "Phones" ("AssetTag");
CREATE UNIQUE INDEX "IX_Sims_SimNumber" ON "Sims" ("SimNumber");

.exit 1