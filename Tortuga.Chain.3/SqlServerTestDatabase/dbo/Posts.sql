CREATE TABLE [dbo].[Posts]
(
	[Id] INT NOT NULL PRIMARY KEY,
	Title nVarChar(100) not null,
	Content nVarChar(max) not null,
	OwnerId int not null references dbo.Users([Id])
)


           