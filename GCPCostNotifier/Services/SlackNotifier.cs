namespace GCPCostNotifier.Services;

using Microsoft.Extensions.Logging;
using Slack.Webhooks;

public class SlackNotifier(string slackWebhookUrl, HttpClient httpClient, ICharacterService characterService, ILogger<SlackNotifier> logger)
    : ISlackNotifier
{
    public async Task NotifyDailyResultAsync(IList<CostSummary> costSummaries, string projectId, CancellationToken cancellationToken)
    {
        using var slackClient = new SlackClient(slackWebhookUrl, httpClient: httpClient);
        var totalCost = decimal.Ceiling(costSummaries.Select(v => v.SummarizedCost).Sum());

        Log.SendingSlackMessage(logger);
        await slackClient.PostAsync(new SlackMessage
        {
            Text = characterService.GetGreetingMessage(totalCost, projectId),
            Markdown = true,
            Attachments =
            [
                new SlackAttachment()
                {
                    Text = characterService.GetAttachmentText(),
                    Footer = characterService.GetFooterText(),
                    Fields = costSummaries
                        .Where(v => v.SummarizedCost >= 1.0m)
                        .Select(v => new SlackField
                        {
                            Title = $"{v.ServiceName} - {v.ServiceDescription}",
                            Value = v.SummarizedCost.ToJpyStyleString(),
                        })
                        .ToList(),
                    Color = characterService.GetColor()
                }
            ]
        });
        Log.SlackMessageSent(logger);
    }
}
