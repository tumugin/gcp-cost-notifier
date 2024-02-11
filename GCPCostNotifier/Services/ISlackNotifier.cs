namespace GCPCostNotifier.Services;

public interface ISlackNotifier
{
    public Task NotifyDailyResultAsync(IList<CostSummary> costSummaries, CancellationToken cancellationToken);
}
