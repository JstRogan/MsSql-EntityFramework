using System.ComponentModel.DataAnnotations;

namespace AutoSalonEfLesson1.Entities;

public sealed class ServiceHistory
{
    public int Id { get; set; }

    public int CarId { get; set; }
    public Car? Car { get; set; }

    public DateTime ServiceDate { get; set; }

    [Required]
    [MaxLength(500)]
    public required string Description { get; set; }

    public decimal Cost { get; set; }
}
