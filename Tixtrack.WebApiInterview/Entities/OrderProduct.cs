using System.Text.Json.Serialization;

namespace TixTrack.WebApiInterview.Entities;

public record OrderProduct
{
    [JsonIgnore]
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}