DROP TABLE IF EXISTS __EFMigrationsHistory;

CREATE TABLE ReconcileDisposals (
    Imei            TEXT NOT NULL CONSTRAINT PK_ReconcileDisposals PRIMARY KEY,
    StatusMS        TEXT NOT NULL,
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
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Microsoft','435', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Microsoft','640', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Microsoft','640 XL', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Microsoft','650', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Microsoft','650 Dual Sim', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Mobiwire','Aponi', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Nokia','100', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Nokia','105', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Nokia','207', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Nokia','2610', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Nokia','301', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Nokia','3109C', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Nokia','3120', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Nokia','6021', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Nokia','C1-02', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Samsung','A32', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Samsung','A34', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Samsung','A40', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Samsung','J5', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Siemens','A55', 0);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Sony','L1', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Sony','L3', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Sony','L4', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Sony','XZ3', 1);
INSERT INTO SKUs (Manufacturer, Model, TrackedSKU) Values ('Sony','XA2 Ultra', 1);

