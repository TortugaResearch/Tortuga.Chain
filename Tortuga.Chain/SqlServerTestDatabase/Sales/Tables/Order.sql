CREATE TABLE [Sales].[Order]
(
	OrderKey INT NOT NULL IDENTITY PRIMARY KEY,
	CustomerKey INT NOT NULL References Sales.Customer(CustomerKey),
	OrderDate DateTime2 NOT NULL
)
