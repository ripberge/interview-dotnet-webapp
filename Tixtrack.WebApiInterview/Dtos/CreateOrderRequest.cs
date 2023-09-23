namespace TixTrack.WebApiInterview.Dtos;

public record CreateOrderRequest
{
    public List<CreateOrderProductDto> OrderProducts { get; set; } = new(capacity: 0);
}

public record CreateOrderProductDto
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}