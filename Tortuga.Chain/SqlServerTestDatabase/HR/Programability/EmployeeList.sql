CREATE PROCEDURE HR.EmployeeList
AS
	PRINT 'Listing employees';

	SELECT EmployeeKey, FirstName, LastName FROM HR.Employee;
