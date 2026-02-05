using System.ComponentModel.DataAnnotations;

namespace AutoSalonEfLesson2.Entities;

public sealed class Customer
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public required string Name { get; set; }

    [Required]
    [StringLength(120)]
    public required string Email { get; set; }

    public virtual List<Order> Orders { get; set; } = new();
    public virtual List<CarOrder> CarOrders { get; set; } = new();
}
