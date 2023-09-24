using System.ComponentModel.DataAnnotations.Schema;

namespace TixTrack.WebApiInterview.Entities;

public record Order
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string? Id { get; set; }
    public OrderStatus Status { get; set; }
    public DateTimeOffset Created { get; set; }
    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual bool Equals(Order? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id
               && Status == other.Status
               && Created.Equals(other.Created)
               && OrderProducts.SequenceEqual(other.OrderProducts);
    }

    public override int GetHashCode() =>
        HashCode.Combine(Id, (int)Status, Created, OrderProducts);

    public override string ToString() => $"{nameof(Id)}: {Id}, {nameof(Status)}: {Status}, {nameof(Created)}: {Created}, {nameof(OrderProducts)}: [{string.Join(", ", OrderProducts)}]";
}