CREATE DATABASE AutoSalonEfLesson1;
GO

USE AutoSalonEfLesson1;
GO

CREATE TABLE dbo.CarTypes (
    Id int IDENTITY(1,1) NOT NULL,
    Name nvarchar(30) NOT NULL,
    CONSTRAINT PK_CarTypes PRIMARY KEY (Id),
    CONSTRAINT AK_CarTypes_Name UNIQUE (Name)
);

CREATE TABLE dbo.Customers (
    Id int IDENTITY(1,1) NOT NULL,
    Name nvarchar(100) NOT NULL,
    Email nvarchar(100) NOT NULL,
    Phone nvarchar(20) NOT NULL,
    CONSTRAINT PK_Customers PRIMARY KEY (Id),
    CONSTRAINT AK_Customers_Email UNIQUE (Email)
);

CREATE TABLE dbo.Employees (
    Id int IDENTITY(1,1) NOT NULL,
    Name nvarchar(100) NOT NULL,
    Position nvarchar(60) NOT NULL,
    HireDate datetime2 NOT NULL,
    CONSTRAINT PK_Employees PRIMARY KEY (Id),
    CONSTRAINT CK_Employees_HireDate_Min CHECK (HireDate >= '2000-01-01')
);

CREATE TABLE dbo.Cars (
    Id int IDENTITY(1,1) NOT NULL,
    Brand nvarchar(50) NOT NULL,
    Model nvarchar(50) NOT NULL,
    Year int NOT NULL,
    Price decimal(18,2) NOT NULL,
    Vin nvarchar(17) NOT NULL,
    CarTypeId int NOT NULL,
    CONSTRAINT PK_Cars PRIMARY KEY (Id),
    CONSTRAINT AK_Cars_Vin UNIQUE (Vin),
    CONSTRAINT FK_Cars_CarTypes_CarTypeId FOREIGN KEY (CarTypeId) REFERENCES dbo.CarTypes(Id),
    CONSTRAINT CK_Cars_Year_Min CHECK ([Year] >= 2000),
    CONSTRAINT CK_Cars_Price_Positive CHECK ([Price] > 0)
);

CREATE TABLE dbo.Sales (
    Id int IDENTITY(1,1) NOT NULL,
    CarId int NOT NULL,
    CustomerId int NOT NULL,
    EmployeeId int NOT NULL,
    SaleDate datetime2 NOT NULL,
    SalePrice decimal(18,2) NOT NULL,
    CONSTRAINT PK_Sales PRIMARY KEY (Id),
    CONSTRAINT AK_Sales_CarId UNIQUE (CarId),
    CONSTRAINT FK_Sales_Cars_CarId FOREIGN KEY (CarId) REFERENCES dbo.Cars(Id),
    CONSTRAINT FK_Sales_Customers_CustomerId FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id),
    CONSTRAINT FK_Sales_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(Id),
    CONSTRAINT CK_Sales_SalePrice_Positive CHECK ([SalePrice] > 0)
);

CREATE TABLE dbo.ServiceHistory (
    Id int IDENTITY(1,1) NOT NULL,
    CarId int NOT NULL,
    ServiceDate datetime2 NOT NULL,
    Description nvarchar(500) NOT NULL,
    Cost decimal(18,2) NOT NULL,
    CONSTRAINT PK_ServiceHistory PRIMARY KEY (Id),
    CONSTRAINT FK_ServiceHistory_Cars_CarId FOREIGN KEY (CarId) REFERENCES dbo.Cars(Id) ON DELETE CASCADE,
    CONSTRAINT CK_ServiceHistory_Cost_NonNegative CHECK ([Cost] >= 0)
);

INSERT INTO dbo.CarTypes (Name) VALUES ('Sedan'), ('SUV'), ('Hatchback');

INSERT INTO dbo.Employees (Name, Position, HireDate) VALUES
('Alex Johnson', 'Sales Manager', '2021-02-01'),
('Maria Garcia', 'Sales Consultant', '2022-05-10'),
('Ivan Petrov', 'Sales Consultant', '2023-09-05');

INSERT INTO dbo.Customers (Name, Email, Phone) VALUES
('Customer 01', 'customer01@email.com', '111-111-111'),
('Customer 02', 'customer02@email.com', '222-222-222'),
('Customer 03', 'customer03@email.com', '333-333-333'),
('Customer 04', 'customer04@email.com', '444-444-444'),
('Customer 05', 'customer05@email.com', '555-555-555'),
('Customer 06', 'customer06@email.com', '666-666-666'),
('Customer 07', 'customer07@email.com', '777-777-777'),
('Customer 08', 'customer08@email.com', '888-888-888'),
('Customer 09', 'customer09@email.com', '999-999-999'),
('Customer 10', 'customer10@email.com', '101-010-101');

INSERT INTO dbo.Cars (Brand, Model, Year, Price, Vin, CarTypeId) VALUES
('Toyota', 'Camry', 2022, 30000.00, 'JTDBR32E720000001', 1),
('BMW', 'X5', 2023, 60000.00, 'WBAKR010X00000002', 2),
('Mercedes', 'C-Class', 2021, 50000.00, 'WDDGF8AB0EA000003', 1),
('Volkswagen', 'Golf', 2020, 22000.00, 'WVWZZZ1KZAW000004', 3),
('Audi', 'Q7', 2022, 65000.00, 'WAUZZZ4M0ND000005', 2);

INSERT INTO dbo.Sales (CarId, CustomerId, EmployeeId, SaleDate, SalePrice) VALUES
(1, 1, 1, '2024-01-15', 29500.00),
(2, 2, 1, '2024-02-02', 59000.00),
(3, 3, 2, '2024-03-20', 48500.00),
(4, 4, 2, '2024-04-05', 21000.00),
(5, 5, 3, '2024-05-12', 64000.00);

INSERT INTO dbo.ServiceHistory (CarId, ServiceDate, Description, Cost) VALUES
(1, '2024-06-01', 'Oil change', 120.00),
(1, '2024-09-01', 'Tire rotation', 80.00),
(2, '2024-07-10', 'Brake inspection', 150.00),
(3, '2024-08-25', 'Air filter replacement', 60.00),
(4, '2024-10-05', 'Battery replacement', 200.00),
(5, '2024-11-15', 'Diagnostics', 90.00);
GO
