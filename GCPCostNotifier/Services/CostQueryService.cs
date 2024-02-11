namespace GCPCostNotifier.Services;

using System.Globalization;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Extensions;

public class CostQueryService(string projectId, string targetTableName, ILogger<CostQueryService> logger)
    : ICostQueryService
{
    public async Task<IList<CostSummary>> GetYesterdayCostSummaryAsync(DateTimeOffset targetDateTimeOffset,
        CancellationToken cancellationToken)
    {
        // 基準日は必ず現在の日付の0時0分0秒にする
        var targetZonedDateTime = targetDateTimeOffset.ToZonedDateTime();
        var referenceDateTime = targetZonedDateTime.Date.AtStartOfDayInZone(targetZonedDateTime.Zone);

        // 開始日はぴったり1日前の日時に指定、終了日は指定された日時にする
        var startOffsetDateTime = referenceDateTime.Plus(Duration.Negate(Duration.FromDays(1)));
        var endOffsetDateTime = referenceDateTime;

        // パーティションの日付は開始日はUTCに変換した日付の0時0分0秒、終了日は指定された日時にする
        var partitionStartDateTimeUtc =
            startOffsetDateTime.ToOffsetDateTime().InZone(DateTimeZone.Utc).Date.AtStartOfDayInZone(DateTimeZone.Utc);
        var partitionEndDateTimeUtc = endOffsetDateTime.ToOffsetDateTime().InZone(DateTimeZone.Utc);

        var query =
            @"
SELECT
    service.description AS ServiceName,
    sku.description AS ServiceDescription,
    SUM(cost + IFNULL((SELECT SUM(c.amount) FROM UNNEST(credits) AS c), 0.0)) AS SummarizedCost
FROM @targetTableName
WHERE
    _PARTITIONTIME BETWEEN @partitionStartDateTime AND @partitionEndDateTime AND
    usage_start_time >= @startDateTime AND usage_end_time <= @endDateTime
GROUP BY service.description, sku.description
HAVING SummarizedCost > 0
ORDER BY ServiceName, ServiceDescription";

        Log.InitializingBigQueryClient(logger);
        using var bqClient = await BigQueryClient.CreateAsync(projectId);

        Log.CreatingQueryJob(logger, startOffsetDateTime.ToDateTimeOffset(), endOffsetDateTime.ToDateTimeOffset());
        var job = await bqClient.CreateQueryJobAsync(
            query,
            new[]
            {
                new BigQueryParameter("targetTableName", BigQueryDbType.String, targetTableName), new BigQueryParameter(
                    "partitionStartDateTime", BigQueryDbType.Timestamp,
                    partitionStartDateTimeUtc.ToDateTimeOffset()),
                new BigQueryParameter("partitionEndDateTime", BigQueryDbType.Timestamp,
                    partitionEndDateTimeUtc.ToDateTimeOffset()),
                new BigQueryParameter("startDateTime", BigQueryDbType.Timestamp,
                    startOffsetDateTime.ToDateTimeOffset()),
                new BigQueryParameter("endDateTime", BigQueryDbType.Timestamp, endOffsetDateTime.ToDateTimeOffset())
            },
            cancellationToken: cancellationToken
        );

        Log.QueryJobCreated(logger, job.Reference.JobId);

        (await job.PollUntilCompletedAsync(cancellationToken: cancellationToken)).ThrowOnAnyError();

        Log.QueryJobCompleted(logger, job.Reference.JobId);

        var result = await bqClient.GetQueryResultsAsync(job.Reference, cancellationToken: cancellationToken);
        Log.QueryResultFetched(logger, job.Reference.JobId, result.TotalRows ?? 0);

        return result.Select(v => new CostSummary
        {
            ServiceName =
                v["ServiceName"].ToString() ??
                throw new InvalidOperationException("ServiceName should not be null."),
            ServiceDescription =
                v["ServiceDescription"].ToString() ??
                throw new InvalidOperationException("ServiceDescription should not be null."),
            SummarizedCost =
                decimal.Parse(
                    v["SummarizedCost"].ToString() ??
                    throw new InvalidOperationException("SummarizedCost should not be null."),
                    NumberFormatInfo.InvariantInfo
                )
        }).ToArray();
    }
}
