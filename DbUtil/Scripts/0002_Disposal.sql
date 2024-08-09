DROP TABLE IF EXISTS __EFMigrationsHistory;

CREATE TABLE ReconcileDisposals (
    Imei            TEXT NOT NULL CONSTRAINT PK_ReconcileDisposals PRIMARY KEY,
    StatusMS        TEXT NULL,
    StatusPA        TEXT NULL,
    SR              INTEGER NULL,
    Manufacturer    TEXT NOT NULL,
    Model           TEXT NOT NULL,
    TrackedSKU      INTEGER NOT NULL,
    Certificate     INTEGER NULL,
    Action          TEXT NULL
);

CREATE TABLE SKUs (
    Id              INTEGER NOT NULL CONSTRAINT PK_SKU PRIMARY KEY AUTOINCREMENT,
    Manufacturer    TEXT COLLATE NOCASE NOT NULL,
    Model           TEXT COLLATE NOCASE NOT NULL,
    TrackedSKU      INTEGER NOT NULL
);

CREATE UNIQUE INDEX IX_SKU
ON SKUs (Manufacturer, Model);

INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Apple','iPhone 4s', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Apple','iPhone 5', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Apple','iPhone 5s', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Apple','iPhone 6', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Apple','iPhone 6s', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Apple','iPhone 8', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Apple','iPhone SE', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Apple','iPhone SE 2020', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Apple','iPhone SE 2022', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Nokia','3109C', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Samsung','A32', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Samsung','A34', 1);
