namespace TixTrack.WebApiInterview.Dtos;

public record ReadSalesReportResponse
{
    public int OrderCount { get; set; }
    public double TotalSales { get; set; }
}