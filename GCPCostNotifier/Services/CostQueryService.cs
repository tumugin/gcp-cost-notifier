namespace GCPCostNotifier.Services;

using System.Globalization;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Logging;

public class CostQueryService(
    string projectId,
    string targetTableName,
    IDateTimeCalculationService dateTimeCalculationService,
    ILogger<CostQueryService> logger
) : ICostQueryService
{
    public async Task<IList<CostSummary>> GetYesterdayCostSummaryAsync(
        DateTimeOffset targetDateTimeOffset,
        CancellationToken cancellationToken
    )
    {
        var calculatedDateTimes = dateTimeCalculationService.CalculateDateTimeOffsetsForYesterday(targetDateTimeOffset);

        var query =
            @$"
SELECT
    service.description AS ServiceName,
    sku.description AS ServiceDescription,
    SUM(cost + IFNULL((SELECT SUM(c.amount) FROM UNNEST(credits) AS c), 0.0)) AS SummarizedCost
FROM `{targetTableName}`
WHERE
    _PARTITIONTIME BETWEEN @partitionStartDateTime AND @partitionEndDateTime AND
    usage_start_time >= @startDateTime AND usage_end_time <= @endDateTime
GROUP BY service.description, sku.description
HAVING SummarizedCost > 0
ORDER BY ServiceName, ServiceDescription";

        Log.InitializingBigQueryClient(logger);
        using var bqClient = await BigQueryClient.CreateAsync(projectId);

        Log.CreatingQueryJob(
            logger,
            calculatedDateTimes.StartOffsetDateTimeOffset,
            calculatedDateTimes.EndOffsetDateTimeOffset
        );
        var job = await bqClient.CreateQueryJobAsync(
            query,
            new[]
            {
                new BigQueryParameter(
                    "partitionStartDateTime",
                    BigQueryDbType.Timestamp,
                    calculatedDateTimes.PartitionStartDateTimeOffset
                ),
                new BigQueryParameter(
                    "partitionEndDateTime",
                    BigQueryDbType.Timestamp,
                    calculatedDateTimes.PartitionEndDateTimeOffset
                ),
                new BigQueryParameter(
                    "startDateTime",
                    BigQueryDbType.Timestamp,
                    calculatedDateTimes.StartOffsetDateTimeOffset
                ),
                new BigQueryParameter(
                    "endDateTime",
                    BigQueryDbType.Timestamp,
                    calculatedDateTimes.EndOffsetDateTimeOffset
                )
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
                    NumberStyles.Float,
                    NumberFormatInfo.InvariantInfo
                )
        }).ToArray();
    }
}
