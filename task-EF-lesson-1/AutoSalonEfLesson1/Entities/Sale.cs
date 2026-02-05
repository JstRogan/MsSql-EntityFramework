namespace AutoSalonEfLesson1.Entities;

public sealed class Sale
{
    public int Id { get; set; }

    public int CarId { get; set; }
    public Car? Car { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateTime SaleDate { get; set; }

    public decimal SalePrice { get; set; }
}
