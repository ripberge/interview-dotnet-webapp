namespace TixTrack.WebApiInterview.Dtos;

public record ReadSalesReportDto
{
    public DateTimeOffset? OldestDate;
    public DateTimeOffset? NewestDate;

    public void Deconstruct(
        out DateTimeOffset? oldestDate, out DateTimeOffset? newestDate) =>
        (oldestDate, newestDate) = (OldestDate, NewestDate);
}