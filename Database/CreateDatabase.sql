CREATE TABLE BaseReport (
    [Telephone Number], 
    [User Name], 
    [Contract End Date], 
    [Current Talkplan Name], 
    [Handset Manufacturer], 
    [Sim Number], 
    [Connected IMEI], 
    [Last Used IMEI]
);

CREATE TABLE Disposals
(
 	"IMEI" TEXT UNIQUE,
    "Former User" TEXT,
    OEM TEXT,
    Status TEXT,
    "SR Number" INTEGER,
    "Certificate" INTEGER,
    "AssetTag"	TEXT,
	LastUpdate TEXT DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY("IMEI")
);

CREATE TABLE Phones
(
    IMEI TEXT NOT NULL,
    "Phone Number" TEXT,
    "SIM Number" INTEGER,
    "Former User" TEXT,
    Wiped TEXT,
    Status TEXT,
    OEM TEXT,
    "SR Number" INTEGER,
    "Asset Tag" TEXT,
    "New user" TEXT,
    "Notes" TEXT,
	LastUpdate TEXT DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY("IMEI")
);

CREATE TABLE SIMs
(
    "Phone Number" TEXT NOT NULL,
    "SIM Number" TEXT NOT NULL, 
    Status TEXT, 
    SR INTEGER, 
    "Voice Mail" TEXT, 
    "Call Forwarding" TEXT, 
    Notes TEXT, 
    "Asset Tag" TEXT,
	LastUpdate TEXT DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY("Phone number")
 );

CREATE TABLE UpdateHistoryPhones
(
    "Id" INTEGER NOT NULL,
    UpdateType TEXT NOT NULL,
    IMEI TEXT NOT NULL,
    "Phone Number" TEXT,
    "SIM Number" INTEGER,
    "Former User" TEXT,
    Wiped TEXT,
    Status TEXT,
    OEM TEXT,
    "SR Number" INTEGER,
    "Asset Tag" TEXT,
    "New user" TEXT,
    "Notes" TEXT,
	LastUpdate TEXT NOT NULL,
    PRIMARY KEY("Id" AUTOINCREMENT)
);

CREATE TABLE UpdateHistorySIMs
(
	"Id" INTEGER NOT NULL,
	UpdateType TEXT NOT NULL,
    "Phone Number" TEXT NOT NULL,
    "SIM Number" TEXT NOT NULL, 
    Status TEXT, 
    SR INTEGER, 
    "Voice Mail" TEXT, 
    "Call Forwarding" TEXT, 
    Notes TEXT, 
    "Asset Tag" TEXT,
	LastUpdate TEXT NOT NULL,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
