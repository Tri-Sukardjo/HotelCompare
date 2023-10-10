using HotelCompareWebApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.AzureCognitiveSearch;
using AzureAIConfigBase = HotelCompareWebApi.Models.AzureAIConfigBase;

namespace HotelCompareWebApi.Services;

public sealed class SemanticKernelProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public SemanticKernelProvider(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        this._serviceProvider = serviceProvider;
        this._configuration = configuration;
    }
    public IKernel GetCompletionKernel()
    {
        var builder = Kernel.Builder.WithLoggerFactory(this._serviceProvider.GetRequiredService<ILoggerFactory>());

        this.WithAzureOpenAI(builder);

        return builder.Build();
    }

    private KernelBuilder WithAzureOpenAI(KernelBuilder kernelBuilder)
    {
        var azureOpenAITextConfig = this._configuration.GetSection("Services").GetSection("AzureOpenAIText").Get<AzureOpenAITextConfig>();
        var azureOpenAIEmbeddingConfig = this._configuration.GetSection("Services").GetSection("AzureOpenAIEmbedding").Get<AzureOpenAIEmbeddingConfig>();
        var azureCognitiveSearchConfig = this._configuration.GetSection("Services").GetSection("AzureCognitiveSearch").Get<AzureAIConfigBase>();

        return kernelBuilder
            .WithAzureChatCompletionService(azureOpenAITextConfig!.Deployment, azureOpenAITextConfig!.Endpoint, azureOpenAITextConfig!.APIKey)
            .WithAzureTextEmbeddingGenerationService(azureOpenAIEmbeddingConfig!.Deployment, azureOpenAIEmbeddingConfig!.Endpoint, azureOpenAIEmbeddingConfig!.APIKey)
            .WithMemoryStorage(new AzureCognitiveSearchMemoryStore(azureCognitiveSearchConfig!.Endpoint, azureCognitiveSearchConfig!.APIKey));
    }
}
