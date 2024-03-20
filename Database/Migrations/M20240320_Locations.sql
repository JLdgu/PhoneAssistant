
CREATE TABLE Locations (
	Name	        TEXT NOT NULL PRIMARY KEY,
	Address 	    TEXT NOT NULL,
    PrintDate       INTEGER
);

INSERT INTO Locations VALUES('Collection L87',replace(replace('Collection from L87\r\nby {NewUser}\r\nSR {SR}\r\n{PhoneNumber}','\r',char(13)),'\n',char(10)),1);
INSERT INTO Locations VALUES('Collection GMH',replace(replace('Collection from\r\nHardware Room GMH\r\nby {NewUser}\r\nSR {SR}\r\n{PhoneNumber}','\r',char(13)),'\n',char(10)),1);
INSERT INTO Locations VALUES('Esturary House',replace(replace('{NewUser}\r\nEstuary House\r\nCollett Way\r\nNewton Abbot\r\nTQ12 4PH','\r',char(13)),'\n',char(10)),0);
INSERT INTO Locations VALUES('Follaton House',replace(replace('{NewUser}\r\nFollaton House\r\nPlymouth Road\r\nTotnes \r\nTQ9 5RS','\r',char(13)),'\n',char(10)),0);
INSERT INTO Locations VALUES('Phoenix House',replace(replace('{NewUser}\r\nChildren''s Social Care Team\r\nDevon County Council\r\nPhoenix House\r\nPhoenix Lane\r\nTiverton\r\nEX16 6PP','\r',char(13)),'\n',char(10)),0);
INSERT INTO Locations VALUES('Taw View',replace(replace('{NewUser}\r\nTaw View\r\nNorth Walk\r\nBarnstaple\r\nEX31 1EE','\r',char(13)),'\n',char(10)),0);

.exit 1
