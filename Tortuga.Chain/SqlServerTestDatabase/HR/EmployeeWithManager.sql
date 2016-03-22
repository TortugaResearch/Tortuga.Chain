CREATE VIEW HR.EmployeeWithManager
AS
SELECT  e.EmployeeKey ,
        e.FirstName ,
        e.MiddleName ,
        e.LastName ,
        e.Title ,
        e.ManagerKey ,
        e.OfficePhone ,
        e.CellPhone ,
        e.CreatedDate ,
        e.UpdatedDate ,
        m.EmployeeKey AS ManagerEmployeeKey ,
        m.FirstName AS ManagerFirstName ,
        m.MiddleName AS ManagerMiddleName ,
        m.LastName AS ManagerLastName ,
        m.Title AS ManagerTitle ,
        m.ManagerKey AS ManagerManagerKey ,
        m.OfficePhone AS ManagerOfficePhone ,
        m.CellPhone AS ManagerCellPhone ,
        m.CreatedDate AS ManagerCreatedDate ,
        m.UpdatedDate AS ManagerUpdatedDate
FROM    HR.Employee e
        LEFT JOIN HR.Employee m ON m.EmployeeKey = e.ManagerKey;
