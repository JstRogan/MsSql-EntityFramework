using AutoSalonEfLesson1.Data;
using AutoSalonEfLesson1.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonEfLesson1.Application;

public sealed class DealershipRunner
{
    private readonly DealershipContext _db;

    public DealershipRunner(DealershipContext db)
    {
        _db = db;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await ShowSeededCountsAsync(cancellationToken);
        await DemoCrudAsync(cancellationToken);
        await DemoQueriesAsync(cancellationToken);
    }

    private async Task ShowSeededCountsAsync(CancellationToken cancellationToken)
    {
        var customers = await _db.Customers.CountAsync(cancellationToken);
        var cars = await _db.Cars.CountAsync(cancellationToken);
        var employees = await _db.Employees.CountAsync(cancellationToken);
        var sales = await _db.Sales.CountAsync(cancellationToken);
        var service = await _db.ServiceHistory.CountAsync(cancellationToken);

        Console.WriteLine($"Customers: {customers}");
        Console.WriteLine($"Cars: {cars}");
        Console.WriteLine($"Employees: {employees}");
        Console.WriteLine($"Sales: {sales}");
        Console.WriteLine($"ServiceHistory: {service}");
    }

    private async Task DemoCrudAsync(CancellationToken cancellationToken)
    {
        var newType = new CarType
        {
            Name = $"Type-{Guid.NewGuid():N}"[..25]
        };

        _db.CarTypes.Add(newType);
        await _db.SaveChangesAsync(cancellationToken);

        newType.Name = $"Type-{Guid.NewGuid():N}"[..25];
        await _db.SaveChangesAsync(cancellationToken);

        _db.CarTypes.Remove(newType);
        await _db.SaveChangesAsync(cancellationToken);

        var newCustomer = new Customer
        {
            Name = "New Customer",
            Email = $"new.customer.{Guid.NewGuid():N}@email.com",
            Phone = "000-000-000"
        };

        _db.Customers.Add(newCustomer);
        await _db.SaveChangesAsync(cancellationToken);

        newCustomer.Phone = "000-111-222";
        await _db.SaveChangesAsync(cancellationToken);

        _db.Customers.Remove(newCustomer);
        await _db.SaveChangesAsync(cancellationToken);

        var newEmployee = new Employee
        {
            Name = "New Employee",
            Position = "Sales Consultant",
            HireDate = new DateTime(2024, 1, 1)
        };

        _db.Employees.Add(newEmployee);
        await _db.SaveChangesAsync(cancellationToken);

        newEmployee.Position = "Sales Manager";
        await _db.SaveChangesAsync(cancellationToken);

        _db.Employees.Remove(newEmployee);
        await _db.SaveChangesAsync(cancellationToken);

        var typeId = await _db.CarTypes.Select(x => x.Id).FirstAsync(cancellationToken);

        var newCar = new Car
        {
            Brand = "Honda",
            Model = "Civic",
            Year = 2022,
            Price = 25000m,
            Vin = $"HONDA{Guid.NewGuid():N}"[..17],
            CarTypeId = typeId
        };

        _db.Cars.Add(newCar);
        await _db.SaveChangesAsync(cancellationToken);

        newCar.Price = 24000m;
        await _db.SaveChangesAsync(cancellationToken);

        var newService = new ServiceHistory
        {
            CarId = newCar.Id,
            ServiceDate = new DateTime(2024, 12, 1),
            Description = "Pre-sale inspection",
            Cost = 50m
        };

        _db.ServiceHistory.Add(newService);
        await _db.SaveChangesAsync(cancellationToken);

        newService.Cost = 55m;
        await _db.SaveChangesAsync(cancellationToken);

        var customerId = await _db.Customers.Select(x => x.Id).FirstAsync(cancellationToken);
        var employeeId = await _db.Employees.Select(x => x.Id).FirstAsync(cancellationToken);

        var sale = new Sale
        {
            CarId = newCar.Id,
            CustomerId = customerId,
            EmployeeId = employeeId,
            SaleDate = new DateTime(2024, 12, 15),
            SalePrice = 23900m
        };

        _db.Sales.Add(sale);
        await _db.SaveChangesAsync(cancellationToken);

        sale.SalePrice = 23800m;
        await _db.SaveChangesAsync(cancellationToken);

        _db.Sales.Remove(sale);
        await _db.SaveChangesAsync(cancellationToken);

        _db.ServiceHistory.Remove(newService);
        await _db.SaveChangesAsync(cancellationToken);

        _db.Cars.Remove(newCar);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task DemoQueriesAsync(CancellationToken cancellationToken)
    {
        var customerId = 1;

        var carsBoughtByCustomer = await _db.Sales
            .AsNoTracking()
            .Where(s => s.CustomerId == customerId)
            .Select(s => new { s.Car!.Brand, s.Car.Model, s.SaleDate, s.SalePrice })
            .ToListAsync(cancellationToken);

        Console.WriteLine($"Cars bought by customer {customerId}: {carsBoughtByCustomer.Count}");

        var from = new DateTime(2024, 2, 1);
        var to = new DateTime(2024, 4, 30);

        var salesInPeriod = await _db.Sales
            .AsNoTracking()
            .Where(s => s.SaleDate >= from && s.SaleDate <= to)
            .Select(s => new { s.Id, s.SaleDate, s.SalePrice, Customer = s.Customer!.Name, Employee = s.Employee!.Name })
            .OrderBy(s => s.SaleDate)
            .ToListAsync(cancellationToken);

        Console.WriteLine($"Sales in period: {salesInPeriod.Count}");

        var salesByManager = await _db.Sales
            .AsNoTracking()
            .GroupBy(s => new { s.EmployeeId, s.Employee!.Name })
            .Select(g => new { g.Key.EmployeeId, g.Key.Name, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);

        foreach (var item in salesByManager)
        {
            Console.WriteLine($"EmployeeId={item.EmployeeId} Name={item.Name} Sales={item.Count}");
        }
    }
}
