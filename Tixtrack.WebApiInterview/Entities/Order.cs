using System.ComponentModel.DataAnnotations.Schema;

namespace TixTrack.WebApiInterview.Entities;

public record Order
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
