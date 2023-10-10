namespace HotelCompareWebApi.Models;

public class AzureOpenAITextConfig
{
    public const string PropertyName = "AzureOpenAIText";
    public string Endpoint { get; set; } = string.Empty;
    public string APIKey { get; set; } = string.Empty;
    public string Deployment { get; set; } = string.Empty;
    public string Auth { get; set; } = string.Empty;
    public string APIType { get; set; } = string.Empty;
    public int MaxRetry { get; set; } = 0;
}
