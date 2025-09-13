namespace GCPCostNotifier.Services;

public interface ISlackNotifier
{
    public Task NotifyDailyResultAsync(IList<CostSummary> costSummaries, string projectId, CancellationToken cancellationToken);
}
