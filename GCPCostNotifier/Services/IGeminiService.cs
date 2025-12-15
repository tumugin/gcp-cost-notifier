namespace GCPCostNotifier.Services;

public interface IGeminiService
{
    public Task<string> GetGeminiResponseAsync(
        IList<CostSummary> costSummariesYesterday,
        IList<CostSummary> costSummariesDayBeforeYesterday,
        string projectId,
        CancellationToken cancellationToken
    );
}
