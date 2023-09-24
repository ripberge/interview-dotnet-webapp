using System.Net;
using Flurl;
using Flurl.Http;
using TixTrack.WebApiInterview.Dtos;
using Tixtrack.WebApiInterview.EndToEndTests.Controllers.Base;
using Xunit;

namespace Tixtrack.WebApiInterview.EndToEndTests.Controllers;

public class SalesReportControllerTests : ControllerTestBase
{
    [Fact]
    public async Task GetSalesReport_ResponseStatusIsOk()
    {
        var response = await BaseUrl.AppendPathSegment("v1/salesreport").GetAsync();

        Assert.Equal(HttpStatusCode.OK, response.Status());
    }
    
    [Fact]
    public async Task GetSalesReport_OrderCountIsOverTwo()
    {
        var salesReport = await BaseUrl
            .AppendPathSegment("v1/salesreport")
            .GetJsonAsync<ReadSalesReportResponse>();

        Assert.True(salesReport.OrderCount > 2);
    }

    [Fact]
    public async Task GetSalesReport_OldestDateFilterIsNotIgnored()
    {
        var expectedSalesReport = new ReadSalesReportResponse
        {
            OrderCount = 1,
            TotalSales = 14.50
        };
        
        var actualSalesReport = await BaseUrl
            .AppendPathSegment("v1/salesreport")
            .SetQueryParam("oldestDate", "2023-01-02")
            .SetQueryParam("newestDate", "2023-01-05")
            .GetJsonAsync<ReadSalesReportResponse>();

        Assert.Equal(expectedSalesReport, actualSalesReport);
    }
    
    [Fact]
    public async Task GetSalesReport_NewestDateFilterIsNotIgnored()
    {
        var expectedSalesReport = new ReadSalesReportResponse
        {
            OrderCount = 1,
            TotalSales = 10.50
        };
        
        var actualSalesReport = await BaseUrl
            .AppendPathSegment("v1/salesreport")
            .SetQueryParam("newestDate", "2023-01-02")
            .GetJsonAsync<ReadSalesReportResponse>();

        Assert.Equal(expectedSalesReport, actualSalesReport);
    }

    [Fact]
    public async Task GetTopProducts_ResponseStatusIsOk()
    {
        var response = await BaseUrl
            .AppendPathSegment("v1/salesreport/topproducts")
            .GetAsync();
        
        Assert.Equal(HttpStatusCode.OK, response.Status());
    }

    [Fact]
    public async Task GetTopProducts_JsonFieldsAreNotUnmapped()
    {
        var expectedTopProduct =
            new ReadTopProductResponse(Name: "Refrigerator Magnet", Quantity: 5);
        
        var topProducts = await BaseUrl
            .AppendPathSegment("v1/salesreport/topproducts")
            .GetJsonAsync<IList<ReadTopProductResponse>>();
        var actualTopProduct = topProducts.FirstOrDefault();
        
        Assert.Equal(expectedTopProduct, actualTopProduct);
    }
}