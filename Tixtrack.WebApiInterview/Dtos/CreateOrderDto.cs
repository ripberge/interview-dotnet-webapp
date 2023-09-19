namespace TixTrack.WebApiInterview.Dtos;

public record CreateOrderDto
{
    public IEnumerable<CreateOrderProductDto> OrderProducts { get; set; } =
        new List<CreateOrderProductDto>(capacity: 0);
}

public record CreateOrderProductDto
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}