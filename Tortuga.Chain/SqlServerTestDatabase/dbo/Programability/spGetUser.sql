CREATE PROCEDURE [dbo].[spGetUser]
	@Id int 
AS
	SELECT * FROM dbo.Users WHERE Id = @Id;

RETURN 0
