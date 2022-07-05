CREATE FUNCTION Sales.CustomersByStateInline ( @State CHAR(2) )
RETURNS TABLE
AS RETURN
    ( SELECT    CustomerKey ,
                FullName ,
                State ,
                CreatedByKey ,
                UpdatedByKey ,
                CreatedDate ,
                UpdatedDate ,
                DeletedFlag ,
                DeletedDate ,
                DeletedByKey
      FROM      Sales.Customer c
      WHERE     c.State = @State
    );
