using System.Net;
using Flurl;
using Flurl.Http;
using Tixtrack.WebApiInterview.EndToEndTests.Controllers.Base;
using TixTrack.WebApiInterview.Entities;
using Xunit;

namespace Tixtrack.WebApiInterview.EndToEndTests.Controllers;

public partial class ProductControllerTests
{
    [Fact]
    public async Task GetNonExistingProduct_ResponseStatusIsNotFound()
    {
        var response = await BaseUrl
            .AppendPathSegment($"v1/product/{_unknownProductId}")
            .GetAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.Status());
    }
    
    [Fact]
    public async Task GetExistingProduct_ResponseStatusIsOk()
    {
        var response = await BaseUrl
            .AppendPathSegment($"v1/product/{_existingProduct.Id}")
            .GetAsync();

        Assert.Equal(HttpStatusCode.OK, response.Status());
    }

    [Fact]
    public async Task GetProduct_JsonFieldsAreNotUnmapped()
    {
        var expectedProduct = _existingProduct;
        
        var actualProduct = await BaseUrl
            .AppendPathSegment($"v1/product/{expectedProduct.Id}")
            .GetJsonAsync<Product>();

        Assert.Equal(expectedProduct, actualProduct);
    }
}

public partial class ProductControllerTests : ControllerTestBase
{
    private string _unknownProductId => "01HB1VAFM774CBPJFZETS38N9D";
    private Product _existingProduct { get; } = new()
    {
        Id = "01HB2RY5N2DBZWZEDMDQEPXPJ1",
        Name = "Eco-Friendly Water Bottle",
        AvailableQuantity = 200,
        Price = 14.99,
        Type = "Sustainable Goods",
    };
}
