namespace GCPCostNotifier.Test.Services;

using System.Globalization;
using GCPCostNotifier.Services;

public class DateTimeCalculationServiceTest
{
    private DateTimeCalculationService dateTimeCalculationService = null!;

    [SetUp]
    public void Setup()
    {
        this.dateTimeCalculationService = new DateTimeCalculationService();
    }

    [Test]
    public void CalculateDateTimeOffsetsForYesterdayTest()
    {
        var testingDateTimeOffset = DateTimeOffset.Parse("2024-02-12T00:12:10+09:00", CultureInfo.InvariantCulture);
        var losAngelesTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
        var targetDateTimeOffset = TimeZoneInfo.ConvertTime(testingDateTimeOffset, losAngelesTimeZone);

        var result = this.dateTimeCalculationService.CalculateDateTimeOffsetsForYesterday(
            targetDateTimeOffset,
            losAngelesTimeZone
        );

        Assert.Multiple(() =>
        {
            Assert.That(
                result.ReferenceDateTimeOffset.ToString("o", CultureInfo.InvariantCulture),
                Is.EqualTo("2024-02-11T00:00:00.0000000-08:00")
            );
            Assert.That(
                result.StartOffsetDateTimeOffset.ToString("o", CultureInfo.InvariantCulture),
                Is.EqualTo("2024-02-10T00:00:00.0000000-08:00")
            );
            Assert.That(
                result.EndOffsetDateTimeOffset.ToString("o", CultureInfo.InvariantCulture),
                Is.EqualTo("2024-02-11T00:00:00.0000000-08:00")
            );
            Assert.That(
                result.PartitionStartDateTimeOffset.ToString("o", CultureInfo.InvariantCulture),
                Is.EqualTo("2024-02-10T00:00:00.0000000+00:00")
            );
            Assert.That(
                result.PartitionEndDateTimeOffset.ToString("o", CultureInfo.InvariantCulture),
                Is.EqualTo("2024-02-11T08:00:00.0000000+00:00")
            );
        });
    }
}
