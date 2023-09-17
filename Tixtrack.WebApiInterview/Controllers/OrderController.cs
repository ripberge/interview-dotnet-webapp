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
    [Route("")]
    public IEnumerable<Order> GetAll() => _orderService.GetAll();

    [HttpGet]
    [Route("{orderId}")]
    public Order? GetById(int orderId) => _orderService.GetById(orderId);

    [HttpPost]
    [Route("")]
    public int Create(Order order) => _orderService.Create(order);
}
