DELETE FROM Phones
WHERE (Status = 'Decommissioned' AND SRNumber <> 225316)
OR (Status = 'Decommissioned' AND SRNumber IS NULL)
OR Status = 'Disposed';