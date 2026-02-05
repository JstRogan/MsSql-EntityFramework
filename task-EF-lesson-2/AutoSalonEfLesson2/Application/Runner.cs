using System.ComponentModel.DataAnnotations;
using AutoSalonEfLesson2.Data;
using AutoSalonEfLesson2.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonEfLesson2.Application;

public sealed class Runner
{
    private readonly DealershipContext _db;
    private readonly CarService _carService;

    public Runner(DealershipContext db)
    {
        _db = db;
        _carService = new CarService(db);
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await EnsureTestDataAsync(cancellationToken);
        await DemoValidationAsync(cancellationToken);
        await DemoUniqueIndexAsync(cancellationToken);
        await DemoCrudAsync(cancellationToken);
        await DemoEagerLoadingAsync(cancellationToken);
        await DemoExplicitLoadingAsync(cancellationToken);
        await DemoLazyLoadingAsync(cancellationToken);
        await DemoFromSqlRawAsync(cancellationToken);
        await DemoTransactionAsync(cancellationToken);
    }

    private async Task EnsureTestDataAsync(CancellationToken cancellationToken)
    {
        if (await _db.Dealers.AnyAsync(cancellationToken))
            return;

        var d1 = new Dealer { Name = "Dealer One", Location = "Baku" };
        var d2 = new Dealer { Name = "Dealer Two", Location = "Ganja" };

        _db.Dealers.AddRange(d1, d2);

        var c1 = new Car { Make = "Toyota", Model = "Camry", Year = 2022, Dealer = d1 };
        var c2 = new Car { Make = "BMW", Model = "X5", Year = 2023, Dealer = d1 };
        var c3 = new Car { Make = "Mercedes", Model = "C-Class", Year = 2021, Dealer = d2 };

        _db.Cars.AddRange(c1, c2, c3);

        var customer1 = new Customer { Name = "Customer One", Email = "customer1@email.com" };
        var customer2 = new Customer { Name = "Customer Two", Email = "customer2@email.com" };

        _db.Customers.AddRange(customer1, customer2);

        var order1 = new Order { Customer = customer1 };
        var order2 = new Order { Customer = customer2 };

        _db.Orders.AddRange(order1, order2);

        _db.CarOrders.AddRange(
            new CarOrder { Car = c1, Customer = customer1, Order = order1 },
            new CarOrder { Car = c2, Customer = customer1, Order = order1 },
            new CarOrder { Car = c1, Customer = customer2, Order = order2 }
        );

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task DemoValidationAsync(CancellationToken cancellationToken)
    {
        try
        {
            var dealerId = await _db.Dealers.Select(x => x.Id).FirstAsync(cancellationToken);
            var invalid = new Car { Make = "Bad", Model = "Car", Year = 1800, DealerId = dealerId };
            var ctx = new ValidationContext(invalid);
            Validator.ValidateObject(invalid, ctx, validateAllProperties: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task DemoUniqueIndexAsync(CancellationToken cancellationToken)
    {
        try
        {
            var dealerId = await _db.Dealers.Select(x => x.Id).FirstAsync(cancellationToken);
            await _carService.AddAsync(new Car { Make = "Toyota", Model = "Camry", Year = 2022, DealerId = dealerId }, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task DemoCrudAsync(CancellationToken cancellationToken)
    {
        var dealerId = await _db.Dealers.Select(x => x.Id).FirstAsync(cancellationToken);

        var created = await _carService.AddAsync(
            new Car { Make = "Audi", Model = "A6", Year = 2020, DealerId = dealerId },
            cancellationToken);

        Console.WriteLine($"Created Car Id={created.Id}");

        var updated = await _carService.UpdateAsync(created.Id, "Audi", "A6", 2021, dealerId, cancellationToken);
        Console.WriteLine(updated is null ? "Update failed" : $"Updated Car Id={updated.Id}");

        var list = await _carService.GetAllAsync(cancellationToken);
        Console.WriteLine($"Cars (not deleted): {list.Count}");

        var deleted = await _carService.SoftDeleteAsync(created.Id, cancellationToken);
        Console.WriteLine($"Soft deleted: {deleted}");

        var listAfterDelete = await _carService.GetAllAsync(cancellationToken);
        Console.WriteLine($"Cars (not deleted) after delete: {listAfterDelete.Count}");
    }

    private async Task DemoEagerLoadingAsync(CancellationToken cancellationToken)
    {
        var dealers = await _db.Dealers
            .AsNoTracking()
            .Include(d => d.Cars)
            .ToListAsync(cancellationToken);

        foreach (var dealer in dealers)
        {
            Console.WriteLine($"DealerId={dealer.Id} Cars={dealer.Cars.Count}");
        }
    }

    private async Task DemoExplicitLoadingAsync(CancellationToken cancellationToken)
    {
        var car = await _db.Cars.FirstAsync(cancellationToken);

        await _db.Entry(car)
            .Reference(x => x.Dealer)
            .LoadAsync(cancellationToken);

        Console.WriteLine($"Explicit load: CarId={car.Id} Dealer={car.Dealer?.Name}");
    }

    private async Task DemoLazyLoadingAsync(CancellationToken cancellationToken)
    {
        var car = await _db.Cars.FirstAsync(cancellationToken);
        Console.WriteLine($"Lazy load: CarId={car.Id} Dealer={car.Dealer?.Name}");
    }

    private async Task DemoFromSqlRawAsync(CancellationToken cancellationToken)
    {
        var toyotas = await _carService.GetByMakeRawSqlAsync("Toyota", cancellationToken);
        Console.WriteLine($"FromSqlRaw Toyota count: {toyotas.Count}");
    }

    private async Task DemoTransactionAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var dealerId = await _db.Dealers.Select(x => x.Id).FirstAsync(cancellationToken);

            var customer = new Customer
            {
                Name = $"TxCustomer-{Guid.NewGuid():N}"[..18],
                Email = $"tx.{Guid.NewGuid():N}@email.com"[..30]
            };

            var order = new Order { Customer = customer };

            var car = new Car
            {
                Make = $"Make-{Guid.NewGuid():N}"[..12],
                Model = $"Model-{Guid.NewGuid():N}"[..12],
                Year = 2020,
                DealerId = dealerId
            };

            _db.Customers.Add(customer);
            _db.Orders.Add(order);
            _db.Cars.Add(car);
            await _db.SaveChangesAsync(cancellationToken);

            _db.CarOrders.Add(new CarOrder { CarId = car.Id, CustomerId = customer.Id, OrderId = order.Id });
            await _db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            Console.WriteLine("Transaction committed");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            Console.WriteLine(ex.Message);
        }
    }
}
