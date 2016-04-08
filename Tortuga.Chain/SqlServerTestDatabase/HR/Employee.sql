CREATE TABLE HR.Employee
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
      UpdatedDate DATETIME2 NULL
    );

GO

EXEC sys.sp_addextendedproperty @name = N'MS_Description',
    @value = N'Null if the record was never updated.', @level0type = N'SCHEMA',
    @level0name = N'HR', @level1type = N'TABLE', @level1name = N'Employee',
    @level2type = N'COLUMN', @level2name = N'UpdatedDate';