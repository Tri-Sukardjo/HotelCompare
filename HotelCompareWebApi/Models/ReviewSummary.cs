namespace HotelCompareWebApi.Models;

public class ReviewSummary
{
    public List<string> Details { get; set; } = new List<string>();
    public string Summary { get; set; } = string.Empty;
    public double Score { get; set; } = 0;

}
