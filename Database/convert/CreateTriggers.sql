CREATE TRIGGER Disposals_Update
    AFTER UPDATE ON Disposals
    FOR EACH ROW
    WHEN NEW.LastUpdate = OLD.LastUpdate    
BEGIN
   UPDATE Disposals SET LastUpdate = CURRENT_TIMESTAMP WHERE IMEI = old.IMEI;
END;

CREATE TRIGGER Phones_Delete
    AFTER DELETE ON Phones
    FOR EACH ROW
BEGIN
	INSERT INTO UpdateHistoryPhones (UpdateType, IMEI, "Phone Number", "SIM Number", "Former User", Wiped, Status, OEM, "SR Number", "Asset Tag", "New User", Notes, LastUpdate) 
    VALUES ('DELETE', old.IMEI, old."Phone Number", old."SIM Number", old."Former User", old.Wiped, old.Status, old.OEM, old."SR Number", old."Asset Tag", old."New User", old.Notes, old.LastUpdate);
END;

CREATE TRIGGER Phones_Update
    AFTER UPDATE ON Phones
    FOR EACH ROW
    WHEN NEW.LastUpdate = OLD.LastUpdate    
BEGIN
   UPDATE Phones SET LastUpdate = CURRENT_TIMESTAMP WHERE IMEI = old.IMEI;
	INSERT INTO UpdateHistoryPhones (UpdateType, IMEI, "Phone Number", "SIM Number", "Former User", Wiped, Status, OEM, "SR Number", "Asset Tag", "New User", Notes, LastUpdate) 
    VALUES ('UPDATE', old.IMEI, old."Phone Number", old."SIM Number", old."Former User", old.Wiped, old.Status, old.OEM, old."SR Number", old."Asset Tag", old."New User", old.Notes, old.LastUpdate);
END;

CREATE TRIGGER SIMs_Delete
    AFTER DELETE ON SIMs
    FOR EACH ROW
BEGIN
	INSERT INTO UpdateHistorySIMs (UpdateType, "Phone Number", "SIM Number", Status, SR, "Voice Mail", "Call Forwarding", Notes, "Asset Tag", LastUpdate) 
    VALUES ('DELETE', old."Phone Number", old."SIM Number", old.Status, old.SR, old."Voice Mail", old."Call Forwarding", old.Notes, old."Asset Tag", old.LastUpdate);
END;

CREATE TRIGGER SIMs_Update
    AFTER UPDATE ON SIMs
    FOR EACH ROW
    WHEN NEW.LastUpdate = OLD.LastUpdate    
BEGIN
    UPDATE SIMs SET LastUpdate = CURRENT_TIMESTAMP WHERE "Phone Number" = old."Phone Number";
    INSERT INTO UpdateHistorySIMs (UpdateType, "Phone Number", "SIM Number", Status, SR, "Voice Mail", "Call Forwarding", Notes, "Asset Tag", LastUpdate)
    VALUES ('UPDATE', old."Phone Number", old."SIM Number", old.Status, old.SR, old."Voice Mail", old."Call Forwarding", old.Notes, old."Asset Tag", old.LastUpdate);
END;