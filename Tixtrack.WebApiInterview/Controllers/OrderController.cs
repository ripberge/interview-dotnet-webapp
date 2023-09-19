using Microsoft.AspNetCore.Mvc;
using TixTrack.WebApiInterview.Dtos;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Exceptions;
using TixTrack.WebApiInterview.Services;

namespace TixTrack.WebApiInterview.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private IOrderService _orderService { get; set; }
    
    public OrderController(IOrderService orderService) => _orderService = orderService;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> Create(CreateOrderDto orderDto)
    {
        return new ObjectResult(await _orderService.Create(orderDto))
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IList<Order>>> ReadAll()
    {
        var orders = await _orderService.GetAll();
        return orders.Count > 0 ? Ok(orders) : NoContent();
    }

    [HttpGet]
    [Route("{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order?>> ReadById(int orderId)
    {
        var order = await _orderService.GetById(orderId);
        return order == null ? NotFound() : Ok(order);
    }

    [HttpDelete]
    [Route("{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    public async Task<IActionResult> Cancel(int orderId)
    {
        try
        {
            await _orderService.Cancel(orderId);
            return Ok();
        }
        catch (OrderNotFoundException)
        {
            return NotFound();
        }
        catch (OrderIsNotActiveException)
        {
            return StatusCode(StatusCodes.Status412PreconditionFailed, "The order might have been already cancelled.");
        }
    }
}
