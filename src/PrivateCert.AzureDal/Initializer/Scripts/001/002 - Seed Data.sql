INSERT OR REPLACE INTO CertificateTypes(CertificateTypeId, Description) VALUES (1, 'Root');
INSERT OR REPLACE INTO CertificateTypes(CertificateTypeId, Description) VALUES (2, 'Intermediate');
INSERT OR REPLACE INTO CertificateTypes(CertificateTypeId, Description) VALUES (3, 'End-User');

INSERT OR REPLACE INTO Settings(SettingId, Value) VALUES ('MasterKey', NULL);
INSERT OR REPLACE INTO Settings(SettingId, Value) VALUES ('Passphrase', NULL);
