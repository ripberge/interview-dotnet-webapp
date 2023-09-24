using System.Net;
using Flurl;
using Flurl.Http;
using TixTrack.WebApiInterview.Dtos;
using Tixtrack.WebApiInterview.EndToEndTests.Controllers.Base;
using TixTrack.WebApiInterview.Entities;
using Xunit;

namespace Tixtrack.WebApiInterview.EndToEndTests.Controllers;

public partial class OrderControllerTests
{
    [Fact]
    public async Task PostValidOrder_ResponseStatusIsCreated()
    {
        var response = await BaseUrl
            .AppendPathSegment("v1/order")
            .PostJsonAsync(_validCreateRequest);
        
        Assert.Equal(HttpStatusCode.Created, response.Status());
    }
    
    [Fact]
    public async Task PostOrderWithNonPositiveProductQuantity_ResponseStatusIsBadRequest()
    {
        var response = await BaseUrl
            .AppendPathSegment("v1/order")
            .PostJsonAsync(_createRequestWithInvalidProductQuantity);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.Status());
    }
    
    [Fact]
    public async Task PostOrderWithNonExistingProductId_ResponseStatusIsNotFound()
    {
        var response = await BaseUrl
            .AppendPathSegment("v1/order")
            .PostJsonAsync(_createRequestWithInvalidProductId);
        
        Assert.Equal(HttpStatusCode.NotFound, response.Status());
    }
    
    [Fact]
    public async Task GetOrders_ResponseStatusIsOk()
    {
        var response = await BaseUrl.AppendPathSegment("v1/orders").GetAsync();

        Assert.Equal(HttpStatusCode.OK, response.Status());
    }
    
    [Fact]
    public async Task GetOrders_CountIsOverTwo()
    {
        var response = await BaseUrl
            .AppendPathSegment("v1/orders")
            .GetJsonAsync<IList<Order>>();

        Assert.True(response.Count > 2);
    }

    [Fact]
    public async Task GetExistingOrder_ResponseStatusIsOk()
    {
        var expectedOrder = _activeOrder;
        
        var actualOrder = await BaseUrl
            .AppendPathSegment($"v1/order/{expectedOrder.Id}")
            .GetJsonAsync<Order>();

        Assert.Equal(expectedOrder, actualOrder);
    }

    [Fact]
    public async Task GetUnknownOrder_ResponseStatusIsNotFound()
    {
        var response = await BaseUrl
            .AppendPathSegment($"v1/order/{_unknownOrderId}")
            .GetAsync();
        
        Assert.Equal(HttpStatusCode.NotFound, response.Status());
    }

    [Fact]
    public async Task DeleteActiveOrder_ResponseStatusIsOk()
    {
        var activeOrderId = await _createActiveOrder();
        
        var response = await BaseUrl
            .AppendPathSegment($"v1/order/{activeOrderId}")
            .DeleteAsync();
        
        Assert.Equal(HttpStatusCode.OK, response.Status());
    }

    [Fact]
    public async Task DeleteUnknownOrder_ResponseStatusIsNotFound()
    {
        var response = await BaseUrl
            .AppendPathSegment($"v1/order/{_unknownOrderId}")
            .DeleteAsync();
        
        Assert.Equal(HttpStatusCode.NotFound, response.Status());
    }

    [Fact]
    public async Task DeleteInactiveOrder_ResponseStatusIsPreconditionFailed()
    {
        var response = await BaseUrl
            .AppendPathSegment($"v1/order/{_inactiveOrderId}")
            .DeleteAsync();
        
        Assert.Equal(HttpStatusCode.PreconditionFailed, response.Status());
    }
}

public partial class OrderControllerTests : ControllerTestBase
{
    private Order _activeOrder => new()
    {
        Id = "01HAP037A2J4JYFV01S3X8N2SA",
        Status = OrderStatus.Active,
        Created = new DateTimeOffset(new DateTime(2023, 01, 03)),
        OrderProducts = new List<OrderProduct>
        {
            new()
            {
                ProductId = "01HAP09BED95ST5G88HTCC9G9Q",
                Quantity = 2
            }
        }
    };
    private string _inactiveOrderId => "01HB2PKJ5N95FF8GN2TY5CA4TT";
    private string _unknownOrderId => "01HB1VAFM774CBPJFZETS38N9D";
    private string _existingProductId => "01HAP05RW9A0V5Z8NZ57A73JMY";
    private string _unknownProductId => "01HB1VAFM774CBPJFZETS38N9D";
    
    private CreateOrderRequest _createRequestWithInvalidProductQuantity =>
        _getCreateRequest(productId: _existingProductId, quantity: -1);

    private CreateOrderRequest _validCreateRequest =>
        _getCreateRequest(productId: _existingProductId, quantity: 1);

    private CreateOrderRequest _createRequestWithInvalidProductId =>
        _getCreateRequest(productId: _unknownProductId, quantity: 1);
    
    private CreateOrderRequest _getCreateRequest(string productId, int quantity)
    {
        return new CreateOrderRequest
        {
            OrderProducts = new List<OrderProductDto>
            {
                new() { ProductId = productId, Quantity = quantity }
            }
        };
    }

    private Task<string> _createActiveOrder()
    {
        return BaseUrl
            .AppendPathSegment("v1/order")
            .PostJsonAsync(_validCreateRequest)
            .ReceiveString();
    }
}