using System.ComponentModel.DataAnnotations.Schema;

namespace TixTrack.WebApiInterview.Entities;

public class Order
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public int? Product1Id { get; set; }
    public int? Product1Quantiity { get; set; }
    public int? Product2Id { get; set; }
    public int? Product2Quantiity { get; set; }

    //note: this attribute doesn't mean anything for our fake in-memory database
    //but is meant to clarify that these would not be stored in a real database
    //if we were using one such as a relational database with Entity Framework
    [NotMapped]
    public string Product1Name { get; set; } = string.Empty;
    [NotMapped]
    public double Product1Price { get; set; }
    [NotMapped]
    public string Product2Name { get; set; } = string.Empty;
    [NotMapped]
    public double Product2Price { get; set; }
}
