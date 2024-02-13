namespace GCPCostNotifier.Services;

using NodaTime;
using NodaTime.Extensions;

public class DateTimeCalculationService : IDateTimeCalculationService
{
    public CalculatedDateTimeOffsets CalculateDateTimeOffsetsForYesterday(
        DateTimeOffset targetDateTimeOffset,
        TimeZoneInfo targetTimeZoneInfo
    )
    {
        // 基準日は必ず現在の日付の0時0分0秒にする
        var targetZonedDateTime = targetDateTimeOffset.ToZonedDateTime();
        var referenceDateTime = targetZonedDateTime.Date.AtStartOfDayInZone(DateTimeZoneProviders.Tzdb[targetTimeZoneInfo.Id]);

        // 開始日はぴったり1日前の日時に指定、終了日は指定された日時にする
        var startOffsetDateTime = referenceDateTime.Plus(Duration.Negate(Duration.FromDays(1)));
        var endOffsetDateTime = referenceDateTime;

        // パーティションの日付は開始日はUTCに変換した日付の0時0分0秒、終了日は指定された日時にする
        var partitionStartDateTimeUtc =
            startOffsetDateTime.ToOffsetDateTime().InZone(DateTimeZone.Utc).Date.AtStartOfDayInZone(DateTimeZone.Utc);
        var partitionEndDateTimeUtc = endOffsetDateTime.ToOffsetDateTime().InZone(DateTimeZone.Utc);

        return new CalculatedDateTimeOffsets
        {
            ReferenceDateTimeOffset = referenceDateTime.ToDateTimeOffset(),
            StartOffsetDateTimeOffset = startOffsetDateTime.ToDateTimeOffset(),
            EndOffsetDateTimeOffset = endOffsetDateTime.ToDateTimeOffset(),
            PartitionStartDateTimeOffset = partitionStartDateTimeUtc.ToDateTimeOffset(),
            PartitionEndDateTimeOffset = partitionEndDateTimeUtc.ToDateTimeOffset()
        };
    }
}
