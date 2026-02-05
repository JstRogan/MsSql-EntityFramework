USE CarDealership;
GO

CREATE OR ALTER TRIGGER dbo.trg_Cars_PriceHistory
ON dbo.Cars
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.CarPriceHistory (CarId, OldPrice, NewPrice)
    SELECT d.Id, d.Price, i.Price
    FROM inserted i
    JOIN deleted d ON d.Id = i.Id
    WHERE ISNULL(i.Price, -1) <> ISNULL(d.Price, -1);
END
GO

CREATE OR ALTER TRIGGER dbo.trg_Customers_BlockDeleteWithOrders
ON dbo.Customers
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM deleted d
        JOIN dbo.Orders o ON o.CustomerId = d.Id
    )
    BEGIN
        THROW 51001, 'Cannot delete customer: active orders exist.', 1;
        RETURN;
    END

    DELETE c
    FROM dbo.Customers c
    JOIN deleted d ON d.Id = c.Id;
END
GO

CREATE OR ALTER TRIGGER dbo.trg_Orders_LogDeleted
ON dbo.Orders
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.DeletedOrdersLog (OrderId, CustomerId, CarId, OrderDate)
    SELECT d.Id, d.CustomerId, d.CarId, d.OrderDate
    FROM deleted d;
END
GO

CREATE OR ALTER TRIGGER dbo.trg_Cars_DecreasePriceOnYearChange
ON dbo.Cars
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(Year) = 0
        RETURN;

    DECLARE @DiscountRate DECIMAL(10,6) = 0.05;

    UPDATE c
    SET c.Price = c.Price * (1 - @DiscountRate)
    FROM dbo.Cars c
    JOIN inserted i ON i.Id = c.Id
    JOIN deleted d ON d.Id = i.Id
    WHERE ISNULL(i.Year, -1) <> ISNULL(d.Year, -1);
END
GO

CREATE OR ALTER TRIGGER dbo.trg_Orders_BlockDuplicateCustomerCar
ON dbo.Orders
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.Orders o
        JOIN inserted i ON i.CustomerId = o.CustomerId AND i.CarId = o.CarId
        GROUP BY o.CustomerId, o.CarId
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51002, 'Cannot create duplicate order for the same customer and car.', 1;
        ROLLBACK TRANSACTION;
        RETURN;
    END
END
GO
