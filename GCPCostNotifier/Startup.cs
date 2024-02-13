using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(GCPCostNotifier.Startup))]

namespace GCPCostNotifier;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services;

public class Startup : FunctionsStartup
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        var appSettings = context.Configuration.GetSection("AppSettings").Get<AppSetting>()
                          ?? throw new InvalidOperationException("AppSettings is not configured.");
        services
            .AddHttpClient()
            .AddScoped<IDateTimeCalculationService, DateTimeCalculationService>()
            .AddScoped<ICostQueryService, CostQueryService>(v => new CostQueryService(
                appSettings.ProjectId,
                appSettings.TargetTableName,
                v.GetRequiredService<IDateTimeCalculationService>(),
                v.GetRequiredService<ILogger<CostQueryService>>()
            ))
            .AddScoped<ISlackNotifier, SlackNotifier>(v =>
                new SlackNotifier(
                    appSettings.SlackWebhookUrl,
                    v.GetRequiredService<HttpClient>(),
                    v.GetRequiredService<ILogger<SlackNotifier>>()
                )
            );
        base.ConfigureServices(context, services);
    }
}
