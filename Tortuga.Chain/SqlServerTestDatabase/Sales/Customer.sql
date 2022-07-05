CREATE TABLE Sales.Customer
(
	CustomerKey INT NOT NULL IDENTITY PRIMARY KEY, 
	FullName NVARCHAR(100) NULL,
	State Char(2) NOT NULL,

	CreatedByKey INT NULL REFERENCES HR.Employee(EmployeeKey),
	UpdatedByKey INT NULL REFERENCES HR.Employee(EmployeeKey),

	CreatedDate DATETIME2 NULL,
	UpdatedDate DATETIME2 NULL,

	DeletedFlag BIT NOT NULL Default 0,
	DeletedDate DATETIME2 NULL,
	DeletedByKey INT NULL  REFERENCES HR.Employee(EmployeeKey),

	BirthDay Date NULL,
	PreferredCallTime Time NULL

)

