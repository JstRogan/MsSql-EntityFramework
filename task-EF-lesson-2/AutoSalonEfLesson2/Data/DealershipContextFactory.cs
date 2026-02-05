using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoSalonEfLesson2.Data;

public sealed class DealershipContextFactory : IDesignTimeDbContextFactory<DealershipContext>
{
    public DealershipContext CreateDbContext(string[] args)
    {
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

        return new DealershipContext(options);
    }
}
