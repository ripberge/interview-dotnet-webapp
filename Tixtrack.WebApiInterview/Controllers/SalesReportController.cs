using Microsoft.AspNetCore.Mvc;
using TixTrack.WebApiInterview.Dtos;
using TixTrack.WebApiInterview.Services;

namespace TixTrack.WebApiInterview.Controllers;

[ApiController]
public class SalesReportController : ControllerBase
{
    private ISalesReportService _salesReportService { get; set; }

    public SalesReportController(ISalesReportService salesReportService) =>
        _salesReportService = salesReportService;

    [HttpGet]
    [Route("Order/salesreport")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ReadSalesReportResponse>> Read(
        [FromQuery] ReadSalesReportRequest request)
    {
        return Ok(await _salesReportService.Compute(request));
    }

    [HttpGet]
    [Route("Order/salesreport/topproducts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IList<ReadTopProductResponse>>> ReadTopProducts() =>
        Ok(await _salesReportService.GetTopTenProducts());
}