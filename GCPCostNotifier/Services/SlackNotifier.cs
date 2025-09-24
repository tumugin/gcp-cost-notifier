namespace GCPCostNotifier.Services;

using Microsoft.Extensions.Logging;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.WebApi;

public class SlackNotifier(
    string slackWebhookUrl,
    HttpClient httpClient,
    ICharacterService characterService,
    ILogger<SlackNotifier> logger
) : ISlackNotifier
{
    private SlackApiClient CreateSlackApiClient()
    {
        var jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
        var http = Default.Http(jsonSettings, getHttpClient: () => httpClient);
        var urlBuilder = Default.UrlBuilder(jsonSettings);

        return new SlackApiClient(http, urlBuilder, jsonSettings, null);
    }

    public async Task NotifyDailyResultAsync(
        IList<CostSummary> costSummaries,
        string projectId,
        CancellationToken cancellationToken
    )
    {
        var slack = this.CreateSlackApiClient();
        var totalCost = decimal.Ceiling(costSummaries.Select(v => v.SummarizedCost).Sum());

        Log.SendingSlackMessage(logger);

        var filteredCostSummaries = costSummaries
            .Where(v => v.SummarizedCost >= 1.0m)
            .Select(v => new Markdown
            {
                Text = $"*{v.ServiceName} - {v.ServiceDescription}*\n{v.SummarizedCost.ToJpyStyleString()}"
            })
            .ToArray<TextObject>();

        var blocks = new List<Block>
        {
            new SectionBlock()
            {
                Text = new Markdown { Text = characterService.GetGreetingMessage(totalCost, projectId) },
                Expand = true
            },
            new DividerBlock(),
            new SectionBlock()
            {
                Expand = false, Text = new Markdown { Text = characterService.GetAttachmentText() }
            }
        };

        // Split fields into chunks of 10 and create SectionBlocks
        blocks.AddRange(
            filteredCostSummaries
                .Chunk(10)
                .Select(chunk => new SectionBlock() { Expand = false, Fields = chunk })
        );

        blocks.AddRange([
            new ContextBlock()
            {
                Elements =
                [
                    new Markdown { Text = characterService.GetFooterText() }
                ]
            },
            new ActionsBlock()
            {
                Elements =
                [
                    new Button()
                    {
                        Text = "請求コンソールを見る",
                        Url = $"https://console.cloud.google.com/billing/linkedaccount?project={projectId}"
                    }
                ]
            }
        ]);

        await slack.PostToWebhook(slackWebhookUrl, new Message { Blocks = blocks }, cancellationToken);

        Log.SlackMessageSent(logger);
    }
}
