namespace TixTrack.WebApiInterview.Dtos;

public record SalesReportDto
{
    public int OrderCount { get; set; }
    public double TotalSales { get; set; }
}