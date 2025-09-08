CREATE PROCEDURE dbo.NoColumnNames 
AS

BEGIN
	SELECT (1+1), 'AAA' + 'BBB', 'Foo' AS Test, 'Bar' AS Test, (5*5), 'Tree', 0 AS Last
END
GO