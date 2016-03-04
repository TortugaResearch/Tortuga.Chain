GO


DECLARE @Employee Table
(
	EmployeeKey INT NOT NULL PRIMARY KEY,
	FirstName nvarChar(25) NOT NULL,
	MiddleName nvarChar(25) NULL,
	LastName nVarChar(25) NOT NULL,
	Title nVarChar(100) null,
	ManagerKey INT NULL 
)

INSERT INTO @Employee 
(
	EmployeeKey ,
	FirstName ,
	MiddleName ,
	LastName ,
	Title,
	ManagerKey 
)
VALUES 
(1, 'Richard', 'C.', 'King', 'CEO', null),
(2, 'Patrick', 'G.', 'Kimmell', 'VP Sales', 1),
(3, 'Henry', 'W.', 'Brown', 'VP Finance', 1),
(4, 'Raquel', 'W.', 'Wilkerson', 'VP Operations', 1);


SET Identity_insert HR.Employee on

MERGE INTO HR.Employee t USING @Employee s ON t.EmployeeKey = s.EmployeeKey
WHEN NOT MATCHED THEN INSERT (
	EmployeeKey ,
	FirstName ,
	MiddleName ,
	LastName ,
	Title,
	ManagerKey 
) values (
	EmployeeKey ,
	FirstName ,
	MiddleName ,
	LastName ,
	Title,
	ManagerKey 
);


SET Identity_insert HR.Employee off
