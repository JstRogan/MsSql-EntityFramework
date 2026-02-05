using AutoSalonEfLesson1.Application;
using AutoSalonEfLesson1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("CarDealership");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Connection string 'CarDealership' is missing.");

var options = new DbContextOptionsBuilder<DealershipContext>()
    .UseSqlServer(connectionString)
    .Options;

await using var db = new DealershipContext(options);

try
{
    await db.Database.MigrateAsync();
    var runner = new DealershipRunner(db);
    await runner.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
