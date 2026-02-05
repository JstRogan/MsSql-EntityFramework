using Microsoft.EntityFrameworkCore;

namespace AutoSalonEfLesson2.Entities;

[PrimaryKey(nameof(CarId), nameof(CustomerId))]
public sealed class CarOrder
{
    public int CarId { get; set; }
    public virtual Car? Car { get; set; }

    public int CustomerId { get; set; }
    public virtual Customer? Customer { get; set; }

    public int OrderId { get; set; }
    public virtual Order? Order { get; set; }
}
