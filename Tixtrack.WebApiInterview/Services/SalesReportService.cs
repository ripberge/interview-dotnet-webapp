﻿using TixTrack.WebApiInterview.Dtos;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;

namespace TixTrack.WebApiInterview.Services;

public interface ISalesReportService {
    Task<SalesReportDto> Compute(ReadSalesReportDto dto);
}

public class SalesReportServiceImpl : ISalesReportService
{
    private IOrderRepository _orderRepository { get; set; }
    private IProductRepository _productRepository { get; set; }
 
    public SalesReportServiceImpl(
        IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<SalesReportDto> Compute(ReadSalesReportDto dto)
    {
        return (await _getOrdersSales(dto)).Aggregate(
            seed: new SalesReportDto(),
            func: (salesReport, orderSales) =>
            {
                salesReport.TotalSales += orderSales;
                salesReport.OrderCount++;
                return salesReport;
            });
    }

    private async Task<double[]> _getOrdersSales(ReadSalesReportDto dto)
    {
        var orders = await (dto switch
        {
            ({ } oldestDate, { } newestDate) => _orderRepository.FindActiveWithCreatedDateBetweenDates(oldestDate, newestDate),
            ({ } oldestDate, null) => _orderRepository.FindActiveWithCreatedDateGreaterThan(oldestDate),
            (null, { } newestDate) => _orderRepository.FindActiveWithCreatedDateLessThan(newestDate),
            _ => _orderRepository.FindActive()
        });
        return await Task.WhenAll(orders.Select(GetOrderSales));
    }

    public async Task<double> GetOrderSales(Order order) =>
        (await Task.WhenAll(order.OrderProducts.Select(GetProductSales))).Sum();

    public async Task<double> GetProductSales(OrderProduct orderProduct)
    {
        var product = await _productRepository.FindById(orderProduct.ProductId!);
        return product!.Price * orderProduct.Quantity;
    }
}