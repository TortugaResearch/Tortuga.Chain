﻿CREATE TABLE dbo.MaxTest
(
	[Id] INT NOT NULL IDENTITY CONSTRAINT PK_MaxTest PRIMARY KEY ,
	Ansi VarChar(Max) NULL,
	Uni NVarChar(Max) NULL
)
