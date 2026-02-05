using AutoSalonEfLesson1.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonEfLesson1.Data;

public sealed class DealershipContext : DbContext
{
    public DealershipContext(DbContextOptions<DealershipContext> options) : base(options)
    {
    }

    public DbSet<CarType> CarTypes => Set<CarType>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<ServiceHistory> ServiceHistory => Set<ServiceHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CarType>(b =>
        {
            b.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Car>(b =>
        {
            b.HasIndex(x => x.Vin).IsUnique();
            b.Property(x => x.Price).HasPrecision(18, 2);

            b.HasOne(x => x.CarType)
                .WithMany(x => x.Cars)
                .HasForeignKey(x => x.CarTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Cars_Year_Min", "[Year] >= 2000");
                t.HasCheckConstraint("CK_Cars_Price_Positive", "[Price] > 0");
            });
        });

        modelBuilder.Entity<Customer>(b =>
        {
            b.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Employee>(b =>
        {
            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Employees_HireDate_Min", "[HireDate] >= '2000-01-01'");
            });
        });

        modelBuilder.Entity<Sale>(b =>
        {
            b.Property(x => x.SalePrice).HasPrecision(18, 2);

            b.HasIndex(x => x.CarId).IsUnique();

            b.HasOne(x => x.Customer)
                .WithMany(x => x.Sales)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Employee)
                .WithMany(x => x.Sales)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Car)
                .WithOne(x => x.Sale)
                .HasForeignKey<Sale>(x => x.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Sales_SalePrice_Positive", "[SalePrice] > 0");
                t.HasCheckConstraint("CK_Sales_SaleDate_Min", "[SaleDate] >= '2000-01-01'");
            });
        });

        modelBuilder.Entity<ServiceHistory>(b =>
        {
            b.Property(x => x.Cost).HasPrecision(18, 2);

            b.HasOne(x => x.Car)
                .WithMany(x => x.ServiceHistoryEntries)
                .HasForeignKey(x => x.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ServiceHistory_Cost_NonNegative", "[Cost] >= 0");
                t.HasCheckConstraint("CK_ServiceHistory_ServiceDate_Min", "[ServiceDate] >= '2000-01-01'");
            });
        });

        modelBuilder.Entity<CarType>().HasData(
            new CarType { Id = 1, Name = "Sedan" },
            new CarType { Id = 2, Name = "SUV" },
            new CarType { Id = 3, Name = "Hatchback" }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, Name = "Alex Johnson", Position = "Sales Manager", HireDate = new DateTime(2021, 2, 1) },
            new Employee { Id = 2, Name = "Maria Garcia", Position = "Sales Consultant", HireDate = new DateTime(2022, 5, 10) },
            new Employee { Id = 3, Name = "Ivan Petrov", Position = "Sales Consultant", HireDate = new DateTime(2023, 9, 5) }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Customer 01", Email = "customer01@email.com", Phone = "111-111-111" },
            new Customer { Id = 2, Name = "Customer 02", Email = "customer02@email.com", Phone = "222-222-222" },
            new Customer { Id = 3, Name = "Customer 03", Email = "customer03@email.com", Phone = "333-333-333" },
            new Customer { Id = 4, Name = "Customer 04", Email = "customer04@email.com", Phone = "444-444-444" },
            new Customer { Id = 5, Name = "Customer 05", Email = "customer05@email.com", Phone = "555-555-555" },
            new Customer { Id = 6, Name = "Customer 06", Email = "customer06@email.com", Phone = "666-666-666" },
            new Customer { Id = 7, Name = "Customer 07", Email = "customer07@email.com", Phone = "777-777-777" },
            new Customer { Id = 8, Name = "Customer 08", Email = "customer08@email.com", Phone = "888-888-888" },
            new Customer { Id = 9, Name = "Customer 09", Email = "customer09@email.com", Phone = "999-999-999" },
            new Customer { Id = 10, Name = "Customer 10", Email = "customer10@email.com", Phone = "101-010-101" }
        );

        modelBuilder.Entity<Car>().HasData(
            new Car { Id = 1, Brand = "Toyota", Model = "Camry", Year = 2022, Price = 30000m, Vin = "JTDBR32E720000001", CarTypeId = 1 },
            new Car { Id = 2, Brand = "BMW", Model = "X5", Year = 2023, Price = 60000m, Vin = "WBAKR010X00000002", CarTypeId = 2 },
            new Car { Id = 3, Brand = "Mercedes", Model = "C-Class", Year = 2021, Price = 50000m, Vin = "WDDGF8AB0EA000003", CarTypeId = 1 },
            new Car { Id = 4, Brand = "Volkswagen", Model = "Golf", Year = 2020, Price = 22000m, Vin = "WVWZZZ1KZAW000004", CarTypeId = 3 },
            new Car { Id = 5, Brand = "Audi", Model = "Q7", Year = 2022, Price = 65000m, Vin = "WAUZZZ4M0ND000005", CarTypeId = 2 }
        );

        modelBuilder.Entity<Sale>().HasData(
            new Sale { Id = 1, CarId = 1, CustomerId = 1, EmployeeId = 1, SaleDate = new DateTime(2024, 1, 15), SalePrice = 29500m },
            new Sale { Id = 2, CarId = 2, CustomerId = 2, EmployeeId = 1, SaleDate = new DateTime(2024, 2, 2), SalePrice = 59000m },
            new Sale { Id = 3, CarId = 3, CustomerId = 3, EmployeeId = 2, SaleDate = new DateTime(2024, 3, 20), SalePrice = 48500m },
            new Sale { Id = 4, CarId = 4, CustomerId = 4, EmployeeId = 2, SaleDate = new DateTime(2024, 4, 5), SalePrice = 21000m },
            new Sale { Id = 5, CarId = 5, CustomerId = 5, EmployeeId = 3, SaleDate = new DateTime(2024, 5, 12), SalePrice = 64000m }
        );

        modelBuilder.Entity<ServiceHistory>().HasData(
            new ServiceHistory { Id = 1, CarId = 1, ServiceDate = new DateTime(2024, 6, 1), Description = "Oil change", Cost = 120m },
            new ServiceHistory { Id = 2, CarId = 1, ServiceDate = new DateTime(2024, 9, 1), Description = "Tire rotation", Cost = 80m },
            new ServiceHistory { Id = 3, CarId = 2, ServiceDate = new DateTime(2024, 7, 10), Description = "Brake inspection", Cost = 150m },
            new ServiceHistory { Id = 4, CarId = 3, ServiceDate = new DateTime(2024, 8, 25), Description = "Air filter replacement", Cost = 60m },
            new ServiceHistory { Id = 5, CarId = 4, ServiceDate = new DateTime(2024, 10, 5), Description = "Battery replacement", Cost = 200m },
            new ServiceHistory { Id = 6, CarId = 5, ServiceDate = new DateTime(2024, 11, 15), Description = "Diagnostics", Cost = 90m }
        );
    }
}
