namespace GCPCostNotifier.Services;

public interface IDateTimeCalculationService
{
    public CalculatedDateTimeOffsets CalculateDateTimeOffsetsForYesterday(DateTimeOffset targetDateTimeOffset,
        TimeZoneInfo targetTimeZoneInfo);
}
