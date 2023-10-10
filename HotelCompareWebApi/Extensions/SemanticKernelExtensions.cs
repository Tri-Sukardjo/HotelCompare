using HotelCompareWebApi.Services;
using HotelCompareWebApi.Skills;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Skills.Core;

namespace HotelCompareWebApi.Extensions;

internal static class SemanticKernelExtensions
{
    public delegate Task RegisterSkillsWithKernel(IServiceProvider sp, IKernel kernel);
    public delegate Task KernelSetupHook(IServiceProvider sp, IKernel kernel);

    private static void InitializeKernelProvider(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(sp => new SemanticKernelProvider(sp, builder.Configuration));
    }

    /// <summary>
    /// Add Semantic Kernel services
    /// </summary>
    public static WebApplicationBuilder AddSemanticKernelServices(this WebApplicationBuilder builder)
    {
        builder.InitializeKernelProvider();

        // Semantic Kernel
        builder.Services.AddScoped<IKernel>(
            sp =>
            {
                var provider = sp.GetRequiredService<SemanticKernelProvider>();
                var kernel = provider.GetCompletionKernel();

                sp.GetRequiredService<RegisterSkillsWithKernel>()(sp, kernel);

                // If KernelSetupHook is not null, invoke custom kernel setup.
                sp.GetService<KernelSetupHook>()?.Invoke(sp, kernel);
                return kernel;
            });

        // Register plugins
        builder.Services.AddScoped<RegisterSkillsWithKernel>(sp => RegisterHotelCompareSkillsAsync);

        return builder;
    }

    private static Task RegisterHotelCompareSkillsAsync(IServiceProvider sp, IKernel kernel)
    {
        kernel.RegisterMainSkills(sp);
        kernel.RegisterHelperSkills(sp);
        kernel.ImportSkill(new TextMemorySkill(kernel.Memory)) ;

        return Task.CompletedTask;
    }

    public static IKernel RegisterMainSkills(this IKernel kernel, IServiceProvider sp)
    {
        kernel.ImportSkill(
            new InferSentimentSkill(kernel), 
            "InferSentiment"
            );

        kernel.ImportSkill(
            new ExtractLikesAndDislikesSkill(kernel),
            "ExtractLikesAndDislikes"
            );

        kernel.ImportSkill(
            new GetSummarySkill(kernel),
            "GetSummary"
            );

        kernel.ImportSkill(
            new ConcludeReviewSkill(kernel),
            "ConcludeReview"
            );

        return kernel;
    }

    public static IKernel RegisterHelperSkills(this IKernel kernel, IServiceProvider sp)
    {
        kernel.ImportSkill(
            new ExtractUserPreferenceSkill(kernel),
            "ExtractUserPreference"
            );     

        return kernel;
    }
}


