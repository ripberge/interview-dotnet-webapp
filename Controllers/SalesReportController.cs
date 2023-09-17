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
    public SalesReport GetAllTime() => _salesReportService.GetAllTime();
}