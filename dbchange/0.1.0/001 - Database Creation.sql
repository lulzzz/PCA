CREATE TABLE [dbo].[RootCertificate]
(
	[RootCertificateID] INT NOT NULL IDENTITY (1,1), 
    [ExpirationDate] DATETIME NOT NULL, 
    [SerialNumber] VARCHAR(100) NOT NULL, 
    [Name] VARCHAR(50) NOT NULL, 
    [Thumbprint] VARCHAR(250) NOT NULL, 
    [IssueDate] DATETIME NOT NULL, 
    [RevocationDate] DATETIME NULL, 
    [PfxData] VARBINARY(MAX) NOT NULL, 
    [PfxPassword] VARCHAR(MAX) NULL
)
GO
ALTER TABLE [dbo].[RootCertificate] ADD CONSTRAINT
	PK_RootCertificate PRIMARY KEY CLUSTERED 
	(
	RootCertificateID
	) 
GO
