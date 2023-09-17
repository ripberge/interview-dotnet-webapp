using Microsoft.AspNetCore.Mvc;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Services;

namespace TixTrack.WebApiInterview.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private IOrderService _orderService { get; set; }
    
    public OrderController(IOrderService orderService) => _orderService = orderService;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IList<Order>>> GetAll()
    {
        var orders = await _orderService.GetAll();
        return orders.Count > 0 ? Ok(orders) : NoContent();
    }

    [HttpGet]
    [Route("{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order?>> GetById(int orderId)
    {
        var order = await _orderService.GetById(orderId);
        return order == null ? NotFound() : Ok(order);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> Create(Order order)
    {
        return new ObjectResult(await _orderService.Create(order))
        {
            StatusCode = StatusCodes.Status201Created
        };
    }
}
