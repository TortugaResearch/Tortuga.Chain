CREATE TABLE [dbo].[DoubleKey]
(
	DoubleKeyId INT NOT NULL,
	DoubleKeyCode VARCHAR(5) NOT NULL,
	Value VARCHAR(20) NOT NULL,
	CONSTRAINT PK_DoubleKey PRIMARY KEY (DoubleKeyId, DoubleKeyCode)
)
