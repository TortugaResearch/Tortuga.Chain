CREATE TABLE HR.Employee
	(
	 EmployeeKey INT NOT NULL
					 IDENTITY
					 PRIMARY KEY,
	 FirstName NVARCHAR(25) NOT NULL,
	 MiddleName NVARCHAR(25) NULL,
	 LastName NVARCHAR(25) NOT NULL,
	 Title NVARCHAR(100) NULL,
	 ManagerKey INT NULL
					REFERENCES HR.Employee (EmployeeKey),
	 OfficePhone VARCHAR(15) NULL,
	 CellPhone VARCHAR(15) NULL,
	 CreatedDate DATETIME2 NULL
						   DEFAULT GETDATE()
	)

GO
