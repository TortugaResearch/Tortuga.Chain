CREATE TABLE HR.EmployeeWithTrigger
    (
      EmployeeKey INT NOT NULL
                      IDENTITY
                      PRIMARY KEY ,
      FirstName NVARCHAR(50) NOT NULL ,
      MiddleName NVARCHAR(50) NULL ,
      LastName NVARCHAR(50) NOT NULL ,
      Title NVARCHAR(100) NULL ,
      ManagerKey INT NULL
                     REFERENCES HR.Employee ( EmployeeKey ) ,
      OfficePhone VARCHAR(15) NULL ,
      CellPhone VARCHAR(15) NULL ,
      CreatedDate DATETIME2 NOT NULL
                            DEFAULT GETDATE() ,
      UpdatedDate DATETIME2 NULL,
      EmployeeId NVARCHAR(50) NOT NULL CONSTRAINT UX_EmployeeWithTrigger_EmployeeId UNIQUE, 
      Gender CHAR(1) NOT NULL,
      Status CHAR(1) NULL
    );

GO



CREATE TRIGGER [HR].[Trigger_EmployeeWithTrigger]
    ON [HR].[EmployeeWithTrigger]
    FOR DELETE, INSERT, UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [HR].[EmployeeWithTrigger] SET UpdatedDate =  GETDATE() WHERE EmployeeKey IN (SELECT EmployeeKey FROM Inserted); 
    END