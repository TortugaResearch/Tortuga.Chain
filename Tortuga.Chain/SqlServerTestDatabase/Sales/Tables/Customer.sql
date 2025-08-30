CREATE TABLE Sales.Customer
(
	CustomerKey INT NOT NULL IDENTITY PRIMARY KEY, 
	FullName NVARCHAR(100) NULL,
	State Char(2) NOT NULL,

	CreatedByKey INT NULL REFERENCES HR.Employee(EmployeeKey),
	UpdatedByKey INT NULL REFERENCES HR.Employee(EmployeeKey),

	CreatedDate DATETIME2 NULL,
	UpdatedDate DATETIME2 NULL,

	DeletedFlag BIT NOT NULL Default 0,
	DeletedDate DATETIME2 NULL,
	DeletedByKey INT NULL  REFERENCES HR.Employee(EmployeeKey),

	BirthDay Date NULL,
	PreferredCallTime Time NULL

)


GO
EXEC sp_addextendedproperty @name = N'Known Issues',
    @value = N'What happens if we run out of keys?.',
    @level0type = N'SCHEMA',
    @level0name = N'Sales',
    @level1type = N'TABLE',
    @level1name = N'Customer',
    @level2type = N'COLUMN',
    @level2name = N'CustomerKey'
GO
EXEC sp_addextendedproperty @name = N'Tech Details',
    @value = N'This is an auto-generated column.',
    @level0type = N'SCHEMA',
    @level0name = N'Sales',
    @level1type = N'TABLE',
    @level1name = N'Customer',
    @level2type = N'COLUMN',
    @level2name = N'CustomerKey'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Custtomer''s full name',
    @level0type = N'SCHEMA',
    @level0name = N'Sales',
    @level1type = N'TABLE',
    @level1name = N'Customer',
    @level2type = N'COLUMN',
    @level2name = N'FullName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'All of the US customers are stored here.',
    @level0type = N'SCHEMA',
    @level0name = N'Sales',
    @level1type = N'TABLE',
    @level1name = N'Customer',
    @level2type = NULL,
    @level2name = NULL