namespace GCPCostNotifier.Services;

public class CostSummary
{
    public required string ServiceName { get; init; }
    public required string ServiceDescription { get; init; }
    public required decimal SummarizedCost { get; init; }
}
