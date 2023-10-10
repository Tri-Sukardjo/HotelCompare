namespace HotelCompareWebApi.Models;

public class AzureOpenAIEmbeddingConfig
{
    public const string PropertyName = "AzureOpenAIEmbedding";
    public string Endpoint { get; set; } = string.Empty;
    public string APIKey { get; set; } = string.Empty;
    public string Deployment { get; set; } = string.Empty;
}
