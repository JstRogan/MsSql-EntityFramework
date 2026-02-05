using System.ComponentModel.DataAnnotations;

namespace AutoSalonEfLesson1.Entities;

public sealed class Car
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Brand { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Model { get; set; }

    public int Year { get; set; }

    public decimal Price { get; set; }

    [Required]
    [MaxLength(17)]
    public required string Vin { get; set; }

    public int CarTypeId { get; set; }
    public CarType? CarType { get; set; }

    public Sale? Sale { get; set; }

    public List<ServiceHistory> ServiceHistoryEntries { get; set; } = new();
}
