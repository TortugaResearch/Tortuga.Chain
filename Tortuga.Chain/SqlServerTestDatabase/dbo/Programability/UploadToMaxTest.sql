CREATE PROCEDURE dbo.UploadToMaxTest
	@AnsiLimted VarChar(500)  = null,
	@UniLimited NVarChar(1000)  = null,
	@Ansi VarChar(Max)  = null,
	@Uni NVarChar(Max)  = null
AS
	INSERT INTO dbo.MaxTest (Ansi, Uni) VALUES (@Ansi, @Uni);

SELECT SCOPE_IDENTITY();

RETURN 0;

