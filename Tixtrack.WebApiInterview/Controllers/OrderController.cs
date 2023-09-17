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
    public Task<IList<Order>> GetAll() => _orderService.GetAll();

    [HttpGet]
    [Route("{orderId}")]
    public Task<Order?> GetById(int orderId) => _orderService.GetById(orderId);

    [HttpPost]
    [Route("")]
    public Task<int> Create(Order order) => _orderService.Create(order);
}
