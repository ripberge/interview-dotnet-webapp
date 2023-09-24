namespace TixTrack.WebApiInterview.Dtos;

public record ReadTopProductResponse(string Name, int Quantity)
{
    public string Name { get; set; } = Name;
    public int Quantity { get; set; } = Quantity;
}