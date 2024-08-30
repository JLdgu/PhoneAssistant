DROP TABLE IF EXISTS __EFMigrationsHistory;

CREATE TABLE ReconcileDisposals (
    Name            TEXT NOT NULL CONSTRAINT PK_ReconcileDisposals PRIMARY KEY,
    AssetTag        TEXT NULL,
    SerialNumber    TEXT NULL,
    Status          TEXT NOT NULL,
    Certificate     INTEGER NULL,
    Action          TEXT NULL
);

