using System.ComponentModel.DataAnnotations.Schema;

namespace TixTrack.WebApiInterview.Entities;

public record Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public double Price { get; set; }
    // TODO: Type is a very generic and overused word. Can we come up with a better name? 
    public string Type { get; set; } = string.Empty;

    public virtual bool Equals(Product? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id
               && Name == other.Name
               && AvailableQuantity == other.AvailableQuantity
               && Price.Equals(other.Price)
               && Type == other.Type;
    }

    public override int GetHashCode() =>
        HashCode.Combine(Id, Name, AvailableQuantity, Price, Type);
}
