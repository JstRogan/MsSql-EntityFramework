using DapperCarDealership.Data;
using DapperCarDealership.Models;

var connectionString = Environment.GetEnvironmentVariable("CAR_DEALERSHIP_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Set CAR_DEALERSHIP_CONNECTION_STRING environment variable.");

var repo = new CarRepository(connectionString);

try
{
    var newCar = new Car
    {
        Brand = "Audi",
        Model = "A6",
        Year = 2022,
        Price = 45000m
    };

    var newId = await repo.AddCarAsync(newCar);
    Console.WriteLine($"Inserted car id: {newId}");

    var updated = await repo.UpdateCarPriceAsync(newId, 47000m);
    Console.WriteLine($"Updated rows: {updated}");

    var all = await repo.GetAllCarsAsync();
    Console.WriteLine($"All cars count: {all.Count}");

    var byBrand = await repo.GetCarsByBrandAsync("Audi");
    Console.WriteLine($"Audi cars count: {byBrand.Count}");

    var deleted = await repo.DeleteCarAsync(newId);
    Console.WriteLine($"Deleted rows: {deleted}");

    var transactionCar = new Car
    {
        Brand = "Toyota",
        Model = "Supra",
        Year = 2020,
        Price = 52000m
    };

    await repo.RunTransactionDemoAsync(transactionCar, 49999m, deleteCarId: 1);
    Console.WriteLine("Transaction completed");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
