CREATE TABLE [dbo].[Dog]
(
    Id UNIQUEIDENTIFIER not NULL PRIMARY KEY Default NEWID (),
	Age INT NULL ,
	Name varChar(100) null,
	Weight float null 
)
