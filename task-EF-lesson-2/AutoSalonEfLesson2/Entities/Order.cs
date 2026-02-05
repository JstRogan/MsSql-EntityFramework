namespace AutoSalonEfLesson2.Entities;

public sealed class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public virtual Customer? Customer { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual List<CarOrder> CarOrders { get; set; } = new();
}
