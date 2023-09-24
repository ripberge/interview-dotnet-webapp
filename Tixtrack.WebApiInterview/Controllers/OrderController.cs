using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TixTrack.WebApiInterview.Dtos;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Exceptions;
using TixTrack.WebApiInterview.Services;

namespace TixTrack.WebApiInterview.Controllers;

[ApiController]
[Route("v1")]
public class OrderController : ControllerBase
{
    private IOrderService _orderService { get; set; }
    
    public OrderController(IOrderService orderService) => _orderService = orderService;

    [HttpPost("Order")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> Create(
        [Required] CreateOrderRequest orderRequest)
    {
        try
        {
            return _created(await _orderService.Create(orderRequest));
        }
        catch (InvalidProductIdException e)
        {
            return NotFound(e.Message);
        }
        catch (ProductException e) when (e is InvalidProductQuantityException
                                             or UnavailableProductQuantityException)
        {
            return BadRequest(e.Message);
        }
    }

    private ObjectResult _created(object? value)
    {
        return new ObjectResult(value)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    // TODO: Add sorting and pagination.
    [HttpGet("Orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IList<Order>>> ReadAll()
    {
        var orders = await _orderService.GetAll();
        return orders.Count > 0 ? Ok(orders) : NoContent();
    }

    [HttpGet]
    [Route("Order/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order?>> ReadById(string orderId)
    {
        var order = await _orderService.GetById(orderId);
        return order == null ? NotFound() : Ok(order);
    }

    [HttpDelete]
    [Route("Order/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    public async Task<IActionResult> Cancel(string orderId)
    {
        try
        {
            await _orderService.Cancel(orderId);
            return Ok();
        }
        catch (OrderNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (OrderIsNotActiveException e)
        {
            return StatusCode(StatusCodes.Status412PreconditionFailed, e.Message);
        }
    }
}
