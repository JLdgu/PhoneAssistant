BEGIN TRANSACTION;

CREATE TABLE "ReconcileDisposals" (
    "Imei" TEXT NOT NULL CONSTRAINT "PK_ReconcileDisposals" PRIMARY KEY,
    "StatusMS" TEXT NULL,
    "StatusPA" TEXT NULL,
    "StatusSCC" TEXT NULL,
    "SR" INTEGER NULL,
    "Certificate" INTEGER NULL,
    "Action" TEXT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240513121222_Disposal', '8.0.4');

COMMIT;

