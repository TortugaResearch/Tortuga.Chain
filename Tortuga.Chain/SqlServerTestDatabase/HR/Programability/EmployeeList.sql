CREATE PROCEDURE HR.EmployeeList
AS
	SELECT EmployeeKey, FirstName, LastName FROM HR.Employee;
