
IF NOT EXISTS (SELECT * FROM CertificateTypes WHERE CertificateTypeId = 1)
BEGIN 
	INSERT INTO CertificateTypes VALUES (1, 'Root')
END

IF NOT EXISTS (SELECT * FROM CertificateTypes WHERE CertificateTypeId = 2)
BEGIN 
	INSERT INTO CertificateTypes VALUES (2, 'Intermediate')
END

IF NOT EXISTS (SELECT * FROM CertificateTypes WHERE CertificateTypeId = 3)
BEGIN 
	INSERT INTO CertificateTypes VALUES (3, 'End-User')
END

IF NOT EXISTS (SELECT * FROM Settings WHERE SettingId = 'MasterKey')
BEGIN 
	INSERT INTO Settings VALUES ('MasterKey', NULL)
END

IF NOT EXISTS (SELECT * FROM Settings WHERE SettingId = 'DatabaseVersion')
BEGIN 
	INSERT INTO Settings VALUES ('DatabaseVersion', '1')
END

IF NOT EXISTS (SELECT * FROM Settings WHERE SettingId = 'Passphrase')
BEGIN 
	INSERT INTO Settings VALUES ('Passphrase', NULL)
END