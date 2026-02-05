IF DB_ID(N'CarDealership') IS NULL
BEGIN
    CREATE DATABASE CarDealership;
END
GO

USE CarDealership;
GO

IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) UNIQUE NOT NULL,
        Phone NVARCHAR(20) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Cars', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cars (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Brand NVARCHAR(50) NOT NULL,
        Model NVARCHAR(50) NOT NULL,
        Year INT CHECK (Year >= 2000),
        Price DECIMAL(10,2) CHECK (Price > 0)
    );
END
GO

IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CustomerId INT NOT NULL FOREIGN KEY REFERENCES dbo.Customers(Id) ON DELETE CASCADE,
        CarId INT NOT NULL FOREIGN KEY REFERENCES dbo.Cars(Id) ON DELETE CASCADE,
        OrderDate DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID(N'dbo.CarPriceHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CarPriceHistory (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CarId INT NOT NULL FOREIGN KEY REFERENCES dbo.Cars(Id) ON DELETE CASCADE,
        OldPrice DECIMAL(10,2) NULL,
        NewPrice DECIMAL(10,2) NULL,
        ChangeDate DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID(N'dbo.DeletedOrdersLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DeletedOrdersLog (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NULL,
        CustomerId INT NULL,
        CarId INT NULL,
        OrderDate DATETIME NULL,
        DeletedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE Email = N'ivan.petrov@email.com')
BEGIN
    INSERT INTO dbo.Customers (Name, Email, Phone)
    VALUES (N'Иван Петров', N'ivan.petrov@email.com', N'123-456-789');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE Email = N'maria.sidorova@email.com')
BEGIN
    INSERT INTO dbo.Customers (Name, Email, Phone)
    VALUES (N'Мария Сидорова', N'maria.sidorova@email.com', N'987-654-321');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE Email = N'alex.smirnov@email.com')
BEGIN
    INSERT INTO dbo.Customers (Name, Email, Phone)
    VALUES (N'Алексей Смирнов', N'alex.smirnov@email.com', N'555-666-777');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE Brand = N'Toyota' AND Model = N'Camry' AND Year = 2022)
BEGIN
    INSERT INTO dbo.Cars (Brand, Model, Year, Price)
    VALUES (N'Toyota', N'Camry', 2022, 30000);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE Brand = N'BMW' AND Model = N'X5' AND Year = 2023)
BEGIN
    INSERT INTO dbo.Cars (Brand, Model, Year, Price)
    VALUES (N'BMW', N'X5', 2023, 60000);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE Brand = N'Mercedes' AND Model = N'C-Class' AND Year = 2021)
BEGIN
    INSERT INTO dbo.Cars (Brand, Model, Year, Price)
    VALUES (N'Mercedes', N'C-Class', 2021, 50000);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Orders WHERE CustomerId = 1 AND CarId = 1)
BEGIN
    INSERT INTO dbo.Orders (CustomerId, CarId) VALUES (1, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Orders WHERE CustomerId = 2 AND CarId = 2)
BEGIN
    INSERT INTO dbo.Orders (CustomerId, CarId) VALUES (2, 2);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Orders WHERE CustomerId = 3 AND CarId = 3)
BEGIN
    INSERT INTO dbo.Orders (CustomerId, CarId) VALUES (3, 3);
END
GO
