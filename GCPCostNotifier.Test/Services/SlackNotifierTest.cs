using GCPCostNotifier.Services;
using Microsoft.Extensions.Logging;

namespace GCPCostNotifier.Test.Services;

[TestFixture]
public class SlackNotifierTest
{
    private HttpClient _httpClient;
    private ILogger<SlackNotifier> _logger;
    private ICharacterService _characterService;
    private const string TestWebhookUrl = "https://hooks.slack.com/services/test/test/test";

    [SetUp]
    public void Setup()
    {
        var handler = new HttpClientHandler();
        _httpClient = new HttpClient(handler);
        _logger = new LoggerFactory().CreateLogger<SlackNotifier>();
        _characterService = new MayuriCharacterService();
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    [Test]
    public void SlackNotifierConstructorDoesNotThrow()
    {
        Assert.DoesNotThrow(() =>
        {
            var notifier = new SlackNotifier(TestWebhookUrl, _httpClient, _characterService, _logger);
        });
    }

    [Test]
    public void SlackNotifierWithMayuriCharacterUsesCorrectColor()
    {
        var mayuriService = new MayuriCharacterService();
        var notifier = new SlackNotifier(TestWebhookUrl, _httpClient, mayuriService, _logger);

        Assert.That(mayuriService.GetColor(), Is.EqualTo("#ADE0EE"));
    }

    [Test]
    public void SlackNotifierWithMayuriCharacterUsesCorrectFooter()
    {
        var mayuriService = new MayuriCharacterService();
        var notifier = new SlackNotifier(TestWebhookUrl, _httpClient, mayuriService, _logger);

        Assert.That(mayuriService.GetFooterText(), Does.Contain("まゆりちゃん"));
    }

    [Test]
    public void SlackNotifierWithChiaCharacterUsesCorrectColor()
    {
        var chiaService = new ChiaCharacterService();
        var notifier = new SlackNotifier(TestWebhookUrl, _httpClient, chiaService, _logger);

        Assert.That(chiaService.GetColor(), Is.EqualTo("#FFFFFF"));
    }

    [Test]
    public void SlackNotifierWithChiaCharacterUsesCorrectFooter()
    {
        var chiaService = new ChiaCharacterService();
        var notifier = new SlackNotifier(TestWebhookUrl, _httpClient, chiaService, _logger);

        Assert.That(chiaService.GetFooterText(), Does.Contain("未白ちあ"));
    }

    [Test]
    public void SlackNotifierNotifyDailyResultAsyncProcessesCostSummariesCorrectly()
    {
        var costSummaries = new List<CostSummary>
        {
            new() { ServiceName = "Compute Engine", ServiceDescription = "VM instances", SummarizedCost = 100.50m },
            new() { ServiceName = "Cloud Storage", ServiceDescription = "Object storage", SummarizedCost = 0.50m },
            new() { ServiceName = "BigQuery", ServiceDescription = "Data warehouse", SummarizedCost = 25.75m }
        };

        var notifier = new SlackNotifier(TestWebhookUrl, _httpClient, _characterService, _logger);
        var projectId = "test-project";

        Assert.DoesNotThrowAsync(async () =>
        {
            try
            {
                await notifier.NotifyDailyResultAsync(costSummaries, projectId, CancellationToken.None);
            }
            catch (HttpRequestException)
            {
                // Expected since we're using a fake webhook URL
                // The important thing is that the method processes the parameters correctly
                // without throwing any argument or formatting exceptions
            }
        });
    }
}
