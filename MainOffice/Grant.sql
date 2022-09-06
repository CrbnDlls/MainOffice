IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'IIS APPPOOL\MOPool')
BEGIN
    CREATE LOGIN [IIS APPPOOL\MOPool] 
      FROM WINDOWS WITH DEFAULT_DATABASE=[master], 
      DEFAULT_LANGUAGE=[us_english]
END
GO
CREATE USER [MOUser] 
  FOR LOGIN [IIS APPPOOL\MOPool]
GO
EXEC sp_addrolemember 'db_owner', 'MOUser'
GO