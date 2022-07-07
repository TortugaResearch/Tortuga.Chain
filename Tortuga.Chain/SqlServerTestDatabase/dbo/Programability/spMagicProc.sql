CREATE PROCEDURE [dbo].[spMagicProc]
@a int,
@b INT OUT
AS
	SET @b = 10;
RETURN 5;

