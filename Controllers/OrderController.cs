using Microsoft.AspNetCore.Mvc;

namespace Tixtrack.WebApiInterview;

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
        var orders = Database.GetAllOrders();
        foreach (var o in orders)
        {
            if (o.Product1Id != null)
            {
                Product product = Database.GetProduct(o.Product1Id);
                o.Product1Name = product.Name;
                o.Product1Price = product.Price;
            }
            if (o.Product2Id != null)
            {
                Product product = Database.GetProduct(o.Product2Id);
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
        var orders = Database.GetAllOrders();
        var order = orders.SingleOrDefault(o => o.Id == orderId);
        if (order != null)
        {
            if (order.Product1Id != null)
            {
                Product product = Database.GetProduct(order.Product1Id);
                order.Product1Name = product.Name;
                order.Product1Price = product.Price;
            }
            if (order.Product2Id != null)
            {
                Product product = Database.GetProduct(order.Product2Id);
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
        Database.CreateOrder(order);
        return order.Id;
    }

    public class SalesReport
    {
        public int OrderCount { get; set; }
        public double TotalSales { get; set; }
    }

    [HttpGet]
    [Route("salesreport")]
    public SalesReport GetSalesReport()
    {
        IList<Order> orders = Database.GetAllOrders();
        int OrderCount = 0;
        double totalSales = 0;
        foreach (var o in orders)
        {
            OrderCount++;
            if (o.Product1Id != null)
            {
                Product product = Database.GetProduct(o.Product1Id);
                totalSales += (product.Price * o.Product1Quantiity!.Value);
            }
            if (o.Product2Id != null)
            {
                Product product = Database.GetProduct(o.Product2Id);
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

