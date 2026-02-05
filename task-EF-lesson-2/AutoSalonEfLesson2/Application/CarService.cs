using System.ComponentModel.DataAnnotations;
using AutoSalonEfLesson2.Data;
using AutoSalonEfLesson2.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonEfLesson2.Application;

public sealed class CarService
{
    private readonly DealershipContext _db;

    public CarService(DealershipContext db)
    {
        _db = db;
    }

    public async Task<Car> AddAsync(Car car, CancellationToken cancellationToken = default)
    {
        Validate(car);

        _db.Cars.Add(car);
        await _db.SaveChangesAsync(cancellationToken);
        return car;
    }

    public async Task<Car?> UpdateAsync(int id, string make, string model, int year, int dealerId, CancellationToken cancellationToken = default)
    {
        var car = await _db.Cars.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (car is null)
            return null;

        car.Make = make;
        car.Model = model;
        car.Year = year;
        car.DealerId = dealerId;

        Validate(car);

        await _db.SaveChangesAsync(cancellationToken);
        return car;
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var car = await _db.Cars.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (car is null)
            return false;

        car.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Car>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Cars
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Car>> GetByMakeRawSqlAsync(string make, CancellationToken cancellationToken = default)
    {
        return await _db.Cars
            .FromSqlRaw(
                "SELECT [Id],[Make],[Model],[Year],[IsDeleted],[DealerId] FROM [Cars] WHERE [Make] = {0} AND [IsDeleted] = 0",
                make)
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    private static void Validate(Car car)
    {
        var ctx = new ValidationContext(car);
        Validator.ValidateObject(car, ctx, validateAllProperties: true);
    }
}
