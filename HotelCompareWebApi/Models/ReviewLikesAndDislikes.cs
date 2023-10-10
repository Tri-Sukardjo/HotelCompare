namespace HotelCompareWebApi.Models;

public class ReviewLikesAndDislikes
{
    public List<string> Likes { get; set; } = new List<string>();
    public List<string> Dislikes { get; set; } = new List<string>();
}
