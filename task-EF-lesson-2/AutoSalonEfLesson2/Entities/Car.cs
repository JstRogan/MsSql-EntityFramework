using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonEfLesson2.Entities;

[Index(nameof(Make), nameof(Model), IsUnique = true)]
public sealed class Car
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Make { get; set; }

    [Required]
    [StringLength(100)]
    public required string Model { get; set; }

    [Range(1900, 2100)]
    public int Year { get; set; }

    public bool IsDeleted { get; set; }

    public int DealerId { get; set; }
    public virtual Dealer? Dealer { get; set; }

    public virtual List<CarOrder> CarOrders { get; set; } = new();
}
