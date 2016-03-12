CREATE PROCEDURE Sales.CustomerWithOrdersByState @State CHAR(2)
AS
    SET NOCOUNT ON;

    SELECT  *
    FROM    Sales.Customer c
    WHERE   c.State = @State;

    SELECT  o.*
    FROM    Sales.[Order] o
            INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey
    WHERE   c.State = @State;

    RETURN 0;
