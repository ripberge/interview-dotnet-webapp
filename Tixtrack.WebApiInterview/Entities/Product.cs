using System.ComponentModel.DataAnnotations.Schema;

namespace TixTrack.WebApiInterview.Entities;

public record Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AvailabileQuantity { get; set; }
    public double Price { get; set; }
    public string Type { get; set; } = string.Empty;
}
