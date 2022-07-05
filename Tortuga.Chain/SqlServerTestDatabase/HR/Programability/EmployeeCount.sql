CREATE FUNCTION HR.EmployeeCount (@ManagerKey INT = NULL)
RETURNS INT
AS
	BEGIN

		DECLARE	@Restult INT;
		IF @ManagerKey IS NOT NULL
			SELECT	@Restult = COUNT(*)
			FROM	HR.Employee e
			WHERE	e.ManagerKey = @ManagerKey;
		ELSE
			SELECT	@Restult = COUNT(*)
			FROM	HR.Employee e;

		RETURN @Restult;
	END;
