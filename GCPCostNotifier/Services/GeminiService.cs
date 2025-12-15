namespace GCPCostNotifier.Services;

using Google.GenAI;
using Microsoft.Extensions.Logging;

public class GeminiService(
    string? geminiApiKey,
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
        var costResultString = $"プロジェクトID {projectId} の昨日のコストの詳細:\n";
        foreach (var summary in costSummariesYesterday)
        {
            costResultString += $"- {summary.ServiceName}: {summary.SummarizedCost} JPY\n";
        }

        costResultString += "\n----------\n一昨日のコストの詳細:\n";
        foreach (var summary in costSummariesDayBeforeYesterday)
        {
            costResultString += $"- {summary.ServiceName}: {summary.SummarizedCost} JPY\n";
        }

        prompt += "\n----------\n" + costResultString;

        Log.GeneratingGeminiOutput(logger);

        await using var client = new Client(apiKey: geminiApiKey);
        var result = await client.Models.GenerateContentAsync(
            "gemini-2.5-flash",
            prompt
        );

        Log.GeminiOutputGenerated(logger);

        return result.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ??
               throw new InvalidOperationException("Gemini response is empty.");
    }
}
