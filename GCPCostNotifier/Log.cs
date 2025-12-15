namespace GCPCostNotifier;

using Microsoft.Extensions.Logging;

public static partial class Log
{
    [LoggerMessage(
        message: "Triggered function. The target timezone is `{timezone}` and IsDaylightSavingTime is `{isDaylightSavingTime}`",
        level: LogLevel.Information
    )]
    public static partial void TriggeredFunction(ILogger logger, string timeZone, bool isDaylightSavingTime);

    [LoggerMessage(message: "Initializing BigQuery client.", level: LogLevel.Information)]
    public static partial void InitializingBigQueryClient(ILogger logger);

    [LoggerMessage(
        message: "Creating query job for range between `{startOffsetDateTime}` and `{endOffsetDateTime}`.",
        level: LogLevel.Information
    )]
    public static partial void CreatingQueryJob(
        ILogger logger,
        DateTimeOffset startOffsetDateTime,
        DateTimeOffset endOffsetDateTime
    );

    [LoggerMessage(message: "Query job created for `{jobId}`", level: LogLevel.Information)]
    public static partial void QueryJobCreated(ILogger logger, string jobId);

    [LoggerMessage(message: "Query job completed for `{jobId}`", level: LogLevel.Information)]
    public static partial void QueryJobCompleted(ILogger logger, string jobId);

    [LoggerMessage(message: "Query result fetched for `{jobId}` with {rowCount} rows.", level: LogLevel.Information)]
    public static partial void QueryResultFetched(ILogger logger, string jobId, ulong rowCount);

    [LoggerMessage(message: "Sending Slack message.", level: LogLevel.Information)]
    public static partial void SendingSlackMessage(ILogger logger);

    [LoggerMessage(message: "Slack message sent.", level: LogLevel.Information)]
    public static partial void SlackMessageSent(ILogger logger);

    [LoggerMessage(message: "Gemini output enabled. Fetching additional data for comparison.", level: LogLevel.Information)]
    public static partial void GeminiOutputEnabled(ILogger logger);
}
