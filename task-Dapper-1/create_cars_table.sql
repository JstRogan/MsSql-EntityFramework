USE CarDealership;
GO

IF OBJECT_ID(N'dbo.Cars', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cars (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Brand NVARCHAR(50) NOT NULL,
        Model NVARCHAR(50) NOT NULL,
        Year INT NOT NULL,
        Price DECIMAL(18,2) NOT NULL
    );
END
GO
