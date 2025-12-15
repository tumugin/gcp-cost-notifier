using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GCPCostNotifier.Services;

[assembly: FunctionsStartup(typeof(GCPCostNotifier.Startup))]

namespace GCPCostNotifier;

public class Startup : FunctionsStartup
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services
            .Configure<AppSetting>(context.Configuration.GetSection("AppSettings"))
            .AddHttpClient()
            .AddScoped<IDateTimeCalculationService, DateTimeCalculationService>()
            .AddScoped<ICostQueryService, CostQueryService>(v =>
            {
                var appSettings = v.GetRequiredService<IOptions<AppSetting>>().Value;
                return new CostQueryService(
                    appSettings.ProjectId,
                    appSettings.TargetTableName,
                    appSettings.BillingTargetProjectId ?? appSettings.ProjectId,
                    v.GetRequiredService<IDateTimeCalculationService>(),
                    v.GetRequiredService<ILogger<CostQueryService>>()
                );
            })
            .AddScoped<ICharacterService>(v =>
            {
                var appSettings = v.GetRequiredService<IOptions<AppSetting>>().Value;
                return CharacterServiceFactory.Create(appSettings.Character);
            })
            .AddScoped<ISlackNotifier, SlackNotifier>(v =>
            {
                var appSettings = v.GetRequiredService<IOptions<AppSetting>>().Value;
                return new SlackNotifier(
                    appSettings.SlackWebhookUrl,
                    v.GetRequiredService<HttpClient>(),
                    v.GetRequiredService<ICharacterService>(),
                    v.GetRequiredService<ILogger<SlackNotifier>>()
                );
            })
            .AddScoped<IGeminiService, GeminiService>(v =>
            {
                var appSettings = v.GetRequiredService<IOptions<AppSetting>>().Value;
                return new GeminiService(
                    geminiApiKey: appSettings.GeminiApiKey,
                    characterService: v.GetRequiredService<ICharacterService>(),
                    logger: v.GetRequiredService<ILogger<GeminiService>>()
                );
            });
        base.ConfigureServices(context, services);
    }
}
