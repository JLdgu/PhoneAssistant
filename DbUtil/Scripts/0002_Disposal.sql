DROP TABLE IF EXISTS __EFMigrationsHistory;

CREATE TABLE "ReconcileDisposals" (
    "Imei" TEXT NOT NULL CONSTRAINT "PK_ReconcileDisposals" PRIMARY KEY,
    "StatusMS" TEXT NULL,
    "StatusPA" TEXT NULL,
    "StatusSCC" TEXT NULL,
    "SR" INTEGER NULL,
    "Certificate" INTEGER NULL,
    "Action" TEXT NULL
);
