using Azure.AI.TextAnalytics;
using HotelCompareWebApi.Extensions;
using HotelCompareWebApi.Services;
using System.Text.Json.Serialization;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                   
                });
        });

        builder.Host.AddConfiguration();

        // Configure and add semantic services
        builder
            .AddSemanticKernelServices();

        // configure Azure AI services client
        builder
            .AddTextAnalyticsClientService()
            .AddTextTranslationClientService();

        builder.Services.AddControllers();
        builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.UseCors(MyAllowSpecificOrigins);
        
        app.MapControllers();

        Task runTask = app.RunAsync();
        await runTask;
    }
}