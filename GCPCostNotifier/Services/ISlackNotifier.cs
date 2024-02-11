namespace GCPCostNotifier.Services;

public interface ISlackNotifier
{
    public Task NotifyDailyResult(IList<CostSummary> costSummaries, CancellationToken cancellationToken);
}
