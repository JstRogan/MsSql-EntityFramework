using System.ComponentModel.DataAnnotations;

namespace AutoSalonEfLesson1.Entities;

public sealed class Employee
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(60)]
    public required string Position { get; set; }

    public DateTime HireDate { get; set; }

    public List<Sale> Sales { get; set; } = new();
}
