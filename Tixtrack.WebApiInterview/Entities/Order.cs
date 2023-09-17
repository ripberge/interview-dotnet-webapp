using System.ComponentModel.DataAnnotations.Schema;

namespace TixTrack.WebApiInterview.Entities;

public record Order
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public int? Product1Id { get; set; }
    public int? Product1Quantiity { get; set; }
    public int? Product2Id { get; set; }
    public int? Product2Quantiity { get; set; }
}
