namespace TixTrack.WebApiInterview.Dtos;

public record ReadSalesReportRequest
{
    public DateTimeOffset? OldestDate { get; set; }
    public DateTimeOffset? NewestDate { get; set; }

    public void Deconstruct(
        out DateTimeOffset? oldestDate, out DateTimeOffset? newestDate) =>
        (oldestDate, newestDate) = (OldestDate, NewestDate);
}