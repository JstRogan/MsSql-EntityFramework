USE CarDealership;
GO

DECLARE @CarId INT = (SELECT TOP (1) Id FROM dbo.Cars ORDER BY Id);
DECLARE @CustomerId INT = (SELECT TOP (1) Id FROM dbo.Customers ORDER BY Id);

PRINT 'Test 1: update car price => CarPriceHistory should get a row';
UPDATE dbo.Cars SET Price = Price + 1000 WHERE Id = @CarId;
SELECT TOP (10) * FROM dbo.CarPriceHistory WHERE CarId = @CarId ORDER BY Id DESC;

PRINT 'Test 2: delete customer with orders => should fail';
BEGIN TRY
    DELETE FROM dbo.Customers WHERE Id = 1;
    PRINT 'Unexpected: delete succeeded';
END TRY
BEGIN CATCH
    PRINT ERROR_MESSAGE();
END CATCH

PRINT 'Test 3: delete an order => should be logged';
DECLARE @OrderToDelete INT = (SELECT TOP (1) Id FROM dbo.Orders ORDER BY Id DESC);
DELETE FROM dbo.Orders WHERE Id = @OrderToDelete;
SELECT TOP (10) * FROM dbo.DeletedOrdersLog WHERE OrderId = @OrderToDelete ORDER BY Id DESC;

PRINT 'Test 4: update car year => price should decrease by 5% and be logged';
DECLARE @BeforePrice DECIMAL(10,2) = (SELECT Price FROM dbo.Cars WHERE Id = @CarId);
UPDATE dbo.Cars SET Year = Year + 1 WHERE Id = @CarId;
SELECT @BeforePrice AS BeforePrice, Price AS AfterPrice FROM dbo.Cars WHERE Id = @CarId;
SELECT TOP (10) * FROM dbo.CarPriceHistory WHERE CarId = @CarId ORDER BY Id DESC;

PRINT 'Test 5: duplicate order => should fail';
BEGIN TRY
    INSERT INTO dbo.Orders (CustomerId, CarId)
    SELECT TOP (1) CustomerId, CarId FROM dbo.Orders ORDER BY Id;
    PRINT 'Unexpected: insert succeeded';
END TRY
BEGIN CATCH
    PRINT ERROR_MESSAGE();
END CATCH
GO
