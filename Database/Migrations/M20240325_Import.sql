CREATE TABLE ImportHistory (
    Id              INTEGER NOT NULL PRIMARY KEY,
	Name	        TEXT NOT NULL,
	File     	    TEXT NOT NULL,
    ImportDate      TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

INSERT INTO ImportHistory (Name, File, ImportDate) VALUES ('BaseReport', 'Devon Base Report 190723.xlsx', '2023-07-19 14:13:00');
INSERT INTO ImportHistory (Name, File, ImportDate) VALUES ('BaseReport', 'Devon Base Report 091123.xlsx', '2023-07-19 12:59:00');
INSERT INTO ImportHistory (Name, File, ImportDate) VALUES ('BaseReport', 'Devon Base Report 070224.xlsx', '2024-02-07 09:35:00');
INSERT INTO ImportHistory (Name, File, ImportDate) VALUES ('BaseReport', 'Devon Base Report 060324.xlsx', '2024-02-07 13:22:00');

.exit 1