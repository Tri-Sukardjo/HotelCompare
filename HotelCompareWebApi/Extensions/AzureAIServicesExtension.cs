using Azure;
using Azure.AI.TextAnalytics;
using HotelCompareWebApi.Models;
using Azure.AI.Translation.Text;
using Microsoft.Extensions.DependencyInjection;

namespace HotelCompareWebApi.Extensions;

public static class AzureAIServicesExtension
{
    public static WebApplicationBuilder AddTextAnalyticsClientService(this WebApplicationBuilder builder)
    {
        var azureTextAnalytticsClientConfig = builder.Configuration.GetSection("Services").GetSection("AzureLanguageService").Get<AzureAIConfigBase>();

        builder.Services.AddSingleton(sp => new TextAnalyticsClient(
            new Uri(azureTextAnalytticsClientConfig!.Endpoint), 
            new AzureKeyCredential(azureTextAnalytticsClientConfig!.APIKey)));

        return builder;
    }

    public static WebApplicationBuilder AddTextTranslationClientService(this WebApplicationBuilder builder)
    {
        var azureTextTranslationClientConfig = builder.Configuration.GetSection("Services").GetSection("AzureTranslationService").Get<AzureAITextTranslationConfig>();

        builder.Services.AddSingleton(sp => new TextTranslationClient(
            new AzureKeyCredential(azureTextTranslationClientConfig!.APIKey),
            azureTextTranslationClientConfig.Region));

        return builder;
    }
}
