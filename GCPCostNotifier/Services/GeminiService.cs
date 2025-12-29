namespace GCPCostNotifier.Services;

using Google.GenAI;
using Microsoft.Extensions.Logging;

public class GeminiService(
    string? geminiApiKey,
    string geminiModelName,
    ICharacterService characterService,
    ILogger<GeminiService> logger
) : IGeminiService
{
    public async Task<string> GetGeminiResponseAsync(
        IList<CostSummary> costSummariesYesterday,
        IList<CostSummary> costSummariesDayBeforeYesterday,
        string projectId,
        CancellationToken cancellationToken
    )
    {
        var prompt = characterService.GetGeminiPrompt();
        var yesterdayTotal = costSummariesYesterday.Select(v => v.SummarizedCost).Sum();
        var costResultString = $"プロジェクトID {projectId} の昨日のコスト({yesterdayTotal} JPY)の詳細:\n";
        foreach (var summary in costSummariesYesterday)
        {
            costResultString += $"- {summary.ServiceName}: {summary.SummarizedCost} JPY\n";
        }

        var dayBeforeYesterdayTotal = costSummariesDayBeforeYesterday.Select(v => v.SummarizedCost).Sum();
        costResultString += $"\n----------\n一昨日のコスト({dayBeforeYesterdayTotal} JPY)の詳細:\n";
        foreach (var summary in costSummariesDayBeforeYesterday)
        {
            costResultString += $"- {summary.ServiceName}: {summary.SummarizedCost} JPY\n";
        }

        var totalDiff = yesterdayTotal - dayBeforeYesterdayTotal;
        costResultString +=
            $"\n----------\n昨日のコストは一昨日と比べて {(totalDiff >= 0 ? "増加" : "減少")} しており、その差額は {totalDiff} JPY です。";

        prompt += "\n----------\n" + costResultString;

        Log.GeneratingGeminiOutput(logger);

        await using var client = new Client(apiKey: geminiApiKey);
        var result = await client.Models.GenerateContentAsync(
            geminiModelName,
            prompt
        );

        Log.GeminiOutputGenerated(logger);

        return result.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ??
               throw new InvalidOperationException("Gemini response is empty.");
    }
}
