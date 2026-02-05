using System.ComponentModel.DataAnnotations;

namespace AutoSalonEfLesson2.Entities;

public sealed class Dealer
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public required string Name { get; set; }

    [Required]
    [StringLength(120)]
    public required string Location { get; set; }

    public virtual List<Car> Cars { get; set; } = new();
}
