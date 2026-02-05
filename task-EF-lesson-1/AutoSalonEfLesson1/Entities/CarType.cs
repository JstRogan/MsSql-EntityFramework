using System.ComponentModel.DataAnnotations;

namespace AutoSalonEfLesson1.Entities;

public sealed class CarType
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public required string Name { get; set; }

    public List<Car> Cars { get; set; } = new();
}
