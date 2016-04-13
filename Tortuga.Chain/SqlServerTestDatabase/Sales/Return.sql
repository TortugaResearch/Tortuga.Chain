CREATE TABLE [Sales].[Return]
(
	ReturnKey INT NOT NULL PRIMARY KEY,
	CustomerKey INT NOT NULL References Sales.Customer(CustomerKey)
)
