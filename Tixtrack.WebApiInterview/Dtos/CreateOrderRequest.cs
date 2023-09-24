namespace TixTrack.WebApiInterview.Dtos;

public record CreateOrderRequest
{
    public List<OrderProductDto> OrderProducts { get; set; } = new(capacity: 0);
}

public record OrderProductDto
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}