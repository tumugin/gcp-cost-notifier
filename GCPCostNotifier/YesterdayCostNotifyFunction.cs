using Google.Cloud.Functions.Framework;

namespace GCPCostNotifier;

using CloudNative.CloudEvents;
using Services;

public class YesterdayCostNotifyFunction(ICostQueryService costQueryService, ISlackNotifier slackNotifier) : ICloudEventFunction
{
    public async Task HandleAsync(CloudEvent cloudEvent, CancellationToken cancellationToken)
    {
        var tokyoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
        var targetDateTimeOffset = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, tokyoTimeZone);
        var results = await costQueryService.GetYesterdayCostSummaryAsync(targetDateTimeOffset, cancellationToken);
        await slackNotifier.NotifyDailyResult(results, cancellationToken);
    }
}
