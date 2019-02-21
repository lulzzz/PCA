
/****** Object:  Table [dbo].[Certificate]    Script Date: 14/01/2019 12:38:41 ******/ 
SET ansi_nulls ON 

SET quoted_identifier ON 

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Certificates')
BEGIN
	CREATE TABLE [dbo].[Certificates] 
	  ( 
		 [CertificateId] [INT] IDENTITY(1, 1) NOT NULL, 
		 [ExpirationDate]    [DATETIME] NOT NULL, 
		 [CertificateTypeId] [TINYINT] NOT NULL, 
		 [SerialNumber]      [VARCHAR](100) NOT NULL, 
		 [Name]              [VARCHAR](50) NOT NULL, 
		 [Thumbprint]        [VARCHAR](250) NOT NULL, 
		 [IssueDate]         [DATETIME] NOT NULL, 
		 [RevocationDate]    [DATETIME] NULL, 
		 [PfxData]           [VARBINARY](max) NOT NULL
		 CONSTRAINT [PK_.Certificates] PRIMARY KEY CLUSTERED ( [CertificateId] 
		 ASC )
	  ) 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CertificateTypes')
BEGIN
	CREATE TABLE [dbo].[CertificateTypes] 
	  ( 
		 [CertificateTypeId] [TINYINT] NOT NULL, 
		 [Description]       [VARCHAR](100) NOT NULL, 
		 CONSTRAINT [PK_CertificateTypes] PRIMARY KEY CLUSTERED ( 
		 [CertificateTypeId] ASC )
	  ) 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Logs')
BEGIN
	CREATE TABLE [dbo].[Logs] 
	  ( 
		 [LogId]   [INT] IDENTITY(1, 1) NOT NULL, 
		 [Date]    [DATETIME] NOT NULL, 
		 [Message] [VARCHAR](max) NOT NULL, 
		 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ( [logid] ASC )
	  ) 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Settings')
BEGIN
	CREATE TABLE [dbo].[Settings] 
	  ( 
		 [SettingId]   [VARCHAR](100)  NOT NULL, 
		 [Value]    [VARCHAR](500) NULL
		 CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED ( [SettingId] ASC )
	  ) 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_Certificates_CertificateTypes')
BEGIN
	ALTER TABLE [dbo].[Certificates] 
	  WITH CHECK ADD CONSTRAINT 
	  [FK_Certificates_CertificateTypes] FOREIGN KEY( 
	  [certificatetypeid]) REFERENCES [dbo].[CertificateTypes] ([certificatetypeid]) 
	  ON DELETE CASCADE 

	ALTER TABLE [dbo].[Certificates] 
	  CHECK CONSTRAINT [FK_Certificates_CertificateTypes] 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '_DBVersions')
BEGIN
	CREATE TABLE [_DBVersions](
		[VersionID] [int] NOT NULL,
		[CreationDate] [datetime] NOT NULL
	 CONSTRAINT [PK_Version] PRIMARY KEY CLUSTERED 
	(
		[VersionID] ASC
	))
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '_DBScripts')
BEGIN
	CREATE TABLE [_DBScripts](
		[ScriptID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
		[VersionID] [int] NOT NULL,
		[ScriptName] [varchar](200) NOT NULL,
		[ExecutionDate] [datetime] NOT NULL,
	 CONSTRAINT [PK_DBScripts] PRIMARY KEY CLUSTERED 
	(
		[ScriptID] ASC
	)) 
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_DBScripts_DBVersion')
BEGIN
	ALTER TABLE [_DBScripts]  WITH CHECK ADD  CONSTRAINT [FK_DBScripts_DBVersion] FOREIGN KEY([VersionID])
	REFERENCES [_DBVersions] ([VersionID])
END