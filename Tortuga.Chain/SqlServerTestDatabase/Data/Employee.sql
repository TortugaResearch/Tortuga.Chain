
GO


DECLARE @Employee TABLE
(
    EmployeeKey INT NOT NULL PRIMARY KEY,
    FirstName NVARCHAR(25) NOT NULL,
    MiddleName NVARCHAR(25) NULL,
    LastName NVARCHAR(25) NOT NULL,
    Title NVARCHAR(100) NULL,
    ManagerKey INT NULL
);

INSERT INTO @Employee
(
    EmployeeKey,
    FirstName,
    MiddleName,
    LastName,
    Title,
    ManagerKey
)
VALUES
(1, 'Richard', 'C.', 'King', 'CEO', NULL),
(2, 'Patrick', 'G.', 'Kimmell', 'VP Sales', 1),
(3, 'Henry', 'W.', 'Brown', 'VP Finance', 1),
(4, 'Raquel', 'W.', 'Wilkerson', 'VP Operations', 1);


SET IDENTITY_INSERT HR.Employee ON;

MERGE INTO HR.Employee t
USING @Employee s
ON t.EmployeeKey = s.EmployeeKey
WHEN NOT MATCHED THEN
    INSERT
    (
        EmployeeKey,
        FirstName,
        MiddleName,
        LastName,
        Title,
        ManagerKey,
        EmployeeId
    )
    VALUES
    (s.EmployeeKey, s.FirstName, s.MiddleName, s.LastName, s.Title, s.ManagerKey, s.EmployeeKey --just copy the employee key into the employee id
        );


SET IDENTITY_INSERT HR.Employee OFF;
