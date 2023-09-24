using System.ComponentModel.DataAnnotations.Schema;

namespace TixTrack.WebApiInterview.Entities;

public record Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    // TODO: Are double-precision rounding errors acceptable in this application? Would the decimal data type provide better precision?
    public double Price { get; set; }
    // TODO: Type is a very generic and overused word. Can we come up with a better name? 
    public string Type { get; set; } = string.Empty;
}