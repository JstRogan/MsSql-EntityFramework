using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AutoSalonEfLesson1.Data;

public sealed class DealershipContextFactory : IDesignTimeDbContextFactory<DealershipContext>
{
    public DealershipContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("CarDealership");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'CarDealership' is missing.");

        var options = new DbContextOptionsBuilder<DealershipContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new DealershipContext(options);
    }
}
