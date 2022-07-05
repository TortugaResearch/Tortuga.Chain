CREATE FUNCTION Sales.CustomersByState ( @State CHAR(2) )
RETURNS @returntable TABLE
    (
      CustomerKey INT NOT NULL ,
      FullName NVARCHAR(100) NULL ,
      State CHAR(2) NOT NULL ,
      CreatedByKey INT NULL ,
      UpdatedByKey INT NULL ,
      CreatedDate DATETIME2 NULL ,
      UpdatedDate DATETIME2 NULL ,
      DeletedFlag BIT NOT NULL ,
      DeletedDate DATETIMEOFFSET NULL ,
      DeletedByKey INT NULL
    )
AS
    BEGIN
        INSERT  @returntable
                SELECT  c.CustomerKey ,
                        c.FullName ,
                        c.State ,
                        c.CreatedByKey ,
                        c.UpdatedByKey ,
                        c.CreatedDate ,
                        c.UpdatedDate ,
                        c.DeletedFlag ,
                        c.DeletedDate ,
                        c.DeletedByKey
                FROM    Sales.Customer c
                WHERE   c.State = @State;
        RETURN;
    END;
