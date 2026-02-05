using Dapper;
using DapperCarDealership.Models;
using Microsoft.Data.SqlClient;

namespace DapperCarDealership.Data;

public sealed class CarRepository
{
    private readonly string _connectionString;

    public CarRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> AddCarAsync(Car car, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO dbo.Cars (Brand, Model, Year, Price) VALUES (@Brand, @Model, @Year, @Price); SELECT CAST(SCOPE_IDENTITY() AS int);";

        await using var connection = Db.OpenConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, car, cancellationToken: cancellationToken));
    }

    public async Task<int> UpdateCarPriceAsync(int carId, decimal newPrice, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE dbo.Cars SET Price = @NewPrice WHERE Id = @CarId";

        await using var connection = Db.OpenConnection(_connectionString);
        return await connection.ExecuteAsync(new CommandDefinition(sql, new { CarId = carId, NewPrice = newPrice }, cancellationToken: cancellationToken));
    }

    public async Task<int> DeleteCarAsync(int carId, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM dbo.Cars WHERE Id = @CarId";

        await using var connection = Db.OpenConnection(_connectionString);
        return await connection.ExecuteAsync(new CommandDefinition(sql, new { CarId = carId }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Car>> GetAllCarsAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Brand, Model, Year, Price FROM dbo.Cars ORDER BY Id";

        await using var connection = Db.OpenConnection(_connectionString);
        var cars = await connection.QueryAsync<Car>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return cars.AsList();
    }

    public async Task<IReadOnlyList<Car>> GetCarsByBrandAsync(string brandName, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Brand, Model, Year, Price FROM dbo.Cars WHERE Brand = @BrandName ORDER BY Id";

        await using var connection = Db.OpenConnection(_connectionString);
        var cars = await connection.QueryAsync<Car>(new CommandDefinition(sql, new { BrandName = brandName }, cancellationToken: cancellationToken));
        return cars.AsList();
    }

    public async Task RunTransactionDemoAsync(Car newCar, decimal updatedPrice, int deleteCarId, CancellationToken cancellationToken = default)
    {
        await using var connection = Db.OpenConnection(_connectionString);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            const string insertSql = "INSERT INTO dbo.Cars (Brand, Model, Year, Price) VALUES (@Brand, @Model, @Year, @Price); SELECT CAST(SCOPE_IDENTITY() AS int);";
            var newCarId = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(insertSql, newCar, transaction, cancellationToken: cancellationToken));

            const string updateSql = "UPDATE dbo.Cars SET Price = @NewPrice WHERE Id = @CarId";
            await connection.ExecuteAsync(
                new CommandDefinition(updateSql, new { CarId = newCarId, NewPrice = updatedPrice }, transaction, cancellationToken: cancellationToken));

            const string deleteSql = "DELETE FROM dbo.Cars WHERE Id = @CarId";
            await connection.ExecuteAsync(
                new CommandDefinition(deleteSql, new { CarId = deleteCarId }, transaction, cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
