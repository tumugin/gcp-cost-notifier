using Google.Cloud.Functions.Framework;

namespace GCPCostNotifier;

using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services;

public class YesterdayCostNotifyFunction(
    ILogger<YesterdayCostNotifyFunction> logger,
    ICostQueryService costQueryService,
    ISlackNotifier slackNotifier,
    IOptions<AppSetting> appSettings
) : ICloudEventFunction
{
    public async Task HandleAsync(CloudEvent cloudEvent, CancellationToken cancellationToken)
    {
        // NOTE: GCPの課金データはロサンゼルス標準時で提供され、かつ、夏時間の影響を受ける
        var losAngelesTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
        var targetDateTimeOffset = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, losAngelesTimeZone);
        Log.TriggeredFunction(
            logger,
            losAngelesTimeZone.Id,
            losAngelesTimeZone.IsDaylightSavingTime(targetDateTimeOffset)
        );
        var results = await costQueryService.GetYesterdayCostSummaryAsync(
            targetDateTimeOffset,
            losAngelesTimeZone,
            cancellationToken
        );
        await slackNotifier.NotifyDailyResultAsync(
            results,
            appSettings.Value.BillingTargetProjectId ?? appSettings.Value.ProjectId,
            cancellationToken
        );
    }
}
