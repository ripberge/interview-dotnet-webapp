﻿using Microsoft.EntityFrameworkCore;
using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public interface IOrderRepository
{
    Task<Order> Create(Order order);
    Task<IList<Order>> FindAll();
    Task<IList<Order>> FindActive();
    Task<Order?> FindById(string orderId);
    Task<Order> Save(Order order);
}

public class InMemoryOrderRepository : InMemoryRepository, IOrderRepository
{
    public InMemoryOrderRepository(ApplicationContext db) : base(db)
    {
    }
    
    public override void Seed()
    {
        _seedOrders();
        _seedOrderProducts();
    }

    private void _seedOrders()
    {
        BulkInsertSync(Db.Orders, new List<Order>
        { 
            new()
            {
                Id = "01HANZZ3C3DJ6Z3NTF53SSDAYC",
                Status = OrderStatus.Active,
                Created = Date(2023, 01, 01)
            },
            new() {
                Id = "01HAP01DBV9GZ418GCS4BNRXN5",
                Status = OrderStatus.Active, 
                Created = Date(2023, 01, 02) 
            },
            new()
            {
                Id = "01HAP037A2J4JYFV01S3X8N2SA",
                Status = OrderStatus.Active,
                Created = Date(2023, 01, 03)
            }
        });

        DateTimeOffset Date(int year, int month, int day) =>
            new DateTimeOffset(new DateTime(year, month, day));
    }
    
    private void _seedOrderProducts()
    {
        BulkInsertSync(Db.OrderProducts, new List<OrderProduct>
        {
            new()
            {
                OrderId = "01HANZZ3C3DJ6Z3NTF53SSDAYC",
                ProductId = "01HAP05RW9A0V5Z8NZ57A73JMY",
                Quantity = 1
            },
            new()
            {
                OrderId = "01HAP01DBV9GZ418GCS4BNRXN5",
                ProductId = "01HAP05RW9A0V5Z8NZ57A73JMY",
                Quantity = 2
            },
            new()
            {
                OrderId = "01HAP01DBV9GZ418GCS4BNRXN5",
                ProductId = "01HAP0ARYM63JGXGFB4R5Q1S28",
                Quantity = 5 
            },
            new()
            {
                OrderId = "01HAP037A2J4JYFV01S3X8N2SA",
                ProductId = "01HAP09BED95ST5G88HTCC9G9Q",
                Quantity = 2
            }
        });
    }

    public async Task<Order> Create(Order order)
    {
        await SaveAndDetach(entry: Db.Orders.Add(order));
        return order;
    }
    
    public async Task<IList<Order>> FindAll()
    {
        return await Db.Orders
            .Include(order => order.OrderProducts)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IList<Order>> FindActive()
    {
        return await Db.Orders
            .Include(order => order.OrderProducts)
            .Where(order => order.Status == OrderStatus.Active)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<Order?> FindById(string orderId)
    {
        return Db.Orders
            .Include(order => order.OrderProducts)
            .AsNoTracking()
            .SingleOrDefaultAsync(order => order.Id == orderId);
    }

    public async Task<Order> Save(Order order)
    {
        await SaveAndDetach(entity: order);
        return order;
    }
}