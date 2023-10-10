namespace HotelCompareWebApi.Models;

public class AzureCognitiveSearchConfig
{
    public const string PropertyName = "AzureCognitiveSearch";
    public string Endpoint { get; set; } = string.Empty;
    public string APIKey { get; set; } = string.Empty;
}
