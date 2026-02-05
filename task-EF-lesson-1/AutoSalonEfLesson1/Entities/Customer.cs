using System.ComponentModel.DataAnnotations;

namespace AutoSalonEfLesson1.Entities;

public sealed class Customer
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Email { get; set; }

    [Required]
    [MaxLength(20)]
    public required string Phone { get; set; }

    public List<Sale> Sales { get; set; } = new();
}
