using AutoSalonEfLesson2.Application;
using AutoSalonEfLesson2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("CarDealership");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Connection string 'CarDealership' is missing.");

var options = new DbContextOptionsBuilder<DealershipContext>()
    .UseLazyLoadingProxies()
    .UseSqlServer(connectionString)
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .Options;

await using var db = new DealershipContext(options);

try
{
    try
    {
        await db.Database.MigrateAsync();
    }
    catch
    {
        await db.Database.EnsureCreatedAsync();
    }

    var runner = new Runner(db);
    await runner.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
