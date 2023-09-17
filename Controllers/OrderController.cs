using Microsoft.AspNetCore.Mvc;
using TixTrack.WebApiInterview.Dtos;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;

namespace TixTrack.WebApiInterview.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    public OrderController()
    {
    }

    [HttpGet]
    [Route("")]
    public IEnumerable<Order> GetAll()
    {
        var orders = OrderRepository.GetAllOrders();
        foreach (var o in orders)
        {
            if (o.Product1Id != null)
            {
                Product product = ProductRepository.GetProduct(o.Product1Id);
                o.Product1Name = product.Name;
                o.Product1Price = product.Price;
            }
            if (o.Product2Id != null)
            {
                Product product = ProductRepository.GetProduct(o.Product2Id);
                o.Product2Name = product.Name;
                o.Product2Price = product.Price;
            }
        }
        return orders;
    }

    [HttpGet]
    [Route("{orderId}")]
    public Order? GetById(int orderId)
    {
        var orders = OrderRepository.GetAllOrders();
        var order = orders.SingleOrDefault(o => o.Id == orderId);
        if (order != null)
        {
            if (order.Product1Id != null)
            {
                Product product = ProductRepository.GetProduct(order.Product1Id);
                order.Product1Name = product.Name;
                order.Product1Price = product.Price;
            }
            if (order.Product2Id != null)
            {
                Product product = ProductRepository.GetProduct(order.Product2Id);
                order.Product2Name = product.Name;
                order.Product2Price = product.Price;
            }
        }
        return order;
    }

    [HttpPost]
    [Route("")]
    public int Create(Order order)
    {
        OrderRepository.CreateOrder(order);
        return order.Id;
    }

    [HttpGet]
    [Route("salesreport")]
    public SalesReport GetSalesReport()
    {
        IList<Order> orders = OrderRepository.GetAllOrders();
        int OrderCount = 0;
        double totalSales = 0;
        foreach (var o in orders)
        {
            OrderCount++;
            if (o.Product1Id != null)
            {
                Product product = ProductRepository.GetProduct(o.Product1Id);
                totalSales += (product.Price * o.Product1Quantiity!.Value);
            }
            if (o.Product2Id != null)
            {
                Product product = ProductRepository.GetProduct(o.Product2Id);
                totalSales += (product.Price * o.Product2Quantiity!.Value);
            }
        }

        return new SalesReport
        {
            TotalSales = totalSales,
            OrderCount = OrderCount,
        };
    }
}
