DROP TABLE BaseReport;

CREATE TABLE BaseReport (
	PhoneNumber	    NOT NULL,
	UserName	    NOT NULL,
	ContractEndDate NOT NULL,
	TalkPlan	    NOT NULL,
	Handset	        NOT NULL,
	SIMNumber	    NOT NULL,
	ConnectedIMEI	NOT NULL,
	LastUsedIMEI	NOT NULL,
	PRIMARY KEY(PhoneNumber)
);

.exit 1