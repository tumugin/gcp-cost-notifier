namespace GCPCostNotifier.Services;

using Microsoft.Extensions.Logging;
using Slack.Webhooks;

public class SlackNotifier(string slackWebhookUrl, HttpClient httpClient, ILogger<SlackNotifier> logger)
    : ISlackNotifier
{
    public async Task NotifyDailyResult(IList<CostSummary> costSummaries, CancellationToken cancellationToken)
    {
        using var slackClient = new SlackClient(slackWebhookUrl, httpClient: httpClient);
        var totalCost = decimal.Ceiling(costSummaries.Select(v => v.SummarizedCost).Sum());

        Log.SendingSlackMessage(logger);
        await slackClient.PostAsync(new SlackMessage
        {
            // まゆりちゃんかわいいね...(Twitterアイコン)
            IconUrl = new Uri("https://pbs.twimg.com/profile_images/1719370833620283392/FhWAjCgD_400x400.jpg"),
            // 作者は仔羽まゆりではないので想像で書いてある。実態はただの限界オタクが想像で書いたものである。
            Text = "** :white_heart: まゆりちゃんが昨日のGCPのコストをお知らせします :white_heart: **\n" +
                   $"昨日のGCPのコストは ** {totalCost.ToJpyStyleString()}円 ** だよ！！\n" +
                   "お金使いすぎないで欲しいな！",
            Markdown = true,
            Attachments =
            [
                new SlackAttachment()
                {
                    Text = "昨日のGCPのコストの詳細だよ！",
                    Fields = costSummaries.Select(v => new SlackField
                    {
                        Title = $"{v.ServiceName} - {v.ServiceDescription}",
                        Value = v.SummarizedCost.ToJpyStyleString() + "円",
                    }).ToList(),
                    Color = "#ADE0EE"
                }
            ]
        });
        Log.SlackMessageSent(logger);
    }
}
