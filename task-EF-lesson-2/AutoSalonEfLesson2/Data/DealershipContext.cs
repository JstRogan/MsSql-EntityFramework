using AutoSalonEfLesson2.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonEfLesson2.Data;

public sealed class DealershipContext : DbContext
{
    public DealershipContext(DbContextOptions<DealershipContext> options) : base(options)
    {
    }

    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Dealer> Dealers => Set<Dealer>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<CarOrder> CarOrders => Set<CarOrder>();
}
