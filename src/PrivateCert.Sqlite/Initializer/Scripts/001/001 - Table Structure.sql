CREATE TABLE IF NOT EXISTS CertificateTypes 
( 
	CertificateTypeId INTEGER PRIMARY KEY, 
	Description       TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Certificates 
( 
	CertificateId			INTEGER PRIMARY KEY, 
	ExpirationDate			TEXT NOT NULL, 
	CertificateTypeId		INTEGER NOT NULL, 
	AuthorityId				INTEGER NULL, 
	SerialNumber			TEXT NOT NULL,
	Name					TEXT NOT NULL, 
	Thumbprint				TEXT NOT NULL, 
	IssueDate				TEXT NOT NULL, 
	RevocationDate			TEXT NULL, 
	PfxData					BLOB NOT NULL,
	FOREIGN KEY(CertificateTypeId) REFERENCES CertificateTypes(CertificateTypeId)
	FOREIGN KEY(AuthorityId) REFERENCES Certificates(CertificateId)
);

CREATE TABLE IF NOT EXISTS AuthorityData
( 
	CertificateId INTEGER PRIMARY KEY, 
	FirstP7B		TEXT NOT NULL, 
	SecondP7B		TEXT NULL,
	FOREIGN KEY(CertificateId) REFERENCES Certificates(CertificateId)
);

CREATE TABLE IF NOT EXISTS Settings 
( 
	SettingId TEXT PRIMARY KEY, 
	Value    TEXT NULL
); 
