using System.Text.Json.Serialization;

namespace TixTrack.WebApiInterview.Entities;

public record OrderProduct
{
    [JsonIgnore]
    public string OrderId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}