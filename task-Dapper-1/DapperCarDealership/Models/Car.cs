namespace DapperCarDealership.Models;

public sealed class Car
{
    public int Id { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
}
