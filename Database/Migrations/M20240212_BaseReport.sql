DROP TABLE BaseReport;

CREATE TABLE BaseReport (
	PhoneNumber	    TEXT NOT NULL,
	UserName	    TEXT NOT NULL,
	ContractEndDate TEXT NOT NULL,
	TalkPlan	    TEXT NOT NULL,
	Handset	        TEXT NOT NULL,
	SIMNumber	    TEXT NOT NULL,
	ConnectedIMEI	TEXT NOT NULL,
	LastUsedIMEI	TEXT NOT NULL,
	PRIMARY KEY(PhoneNumber)
);

.exit 1