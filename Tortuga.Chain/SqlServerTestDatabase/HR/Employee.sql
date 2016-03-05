CREATE TABLE [HR].[Employee]
(
	EmployeeKey INT NOT NULL IDENTITY PRIMARY KEY,
	FirstName nvarChar(25) NOT NULL,
	MiddleName nvarChar(25) NULL,
	LastName nVarChar(25) NOT NULL,
	Title nVarChar(100) null,
	ManagerKey INT NULL REferences HR.Employee(EmployeeKey)
)
