using GCPCostNotifier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using System.Net;

namespace GCPCostNotifier.Test.Services;

[TestFixture]
public class SlackNotifierTest
{
    private HttpClient _httpClient;
    private RecordingWebhookHandler _handler;
    private ILogger<SlackNotifier> _logger;
    private ICharacterService _characterService;
    private const string TestWebhookUrl = "https://hooks.slack.com/services/test/test/test";

    [SetUp]
    public void Setup()
    {
        _handler = new RecordingWebhookHandler();
        _httpClient = new HttpClient(_handler, disposeHandler: false);
        _logger = NullLogger<SlackNotifier>.Instance;
        _characterService = new MayuriCharacterService();
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
        _handler?.Dispose();
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
    public async Task SlackNotifierNotifyDailyResultAsyncProcessesCostSummariesCorrectly()
    {
        var costSummaries = new List<CostSummary>
        {
            new() { ServiceName = "Compute Engine", ServiceDescription = "VM instances", SummarizedCost = 100.50m },
            new() { ServiceName = "Cloud Storage", ServiceDescription = "Object storage", SummarizedCost = 0.50m },
            new() { ServiceName = "BigQuery", ServiceDescription = "Data warehouse", SummarizedCost = 25.75m }
        };

        var notifier = new SlackNotifier(TestWebhookUrl, _httpClient, _characterService, _logger);
        var projectId = "test-project";

        await notifier.NotifyDailyResultAsync(costSummaries, projectId, CancellationToken.None);

        var payload = JObject.Parse(_handler.Body!);
        var fields = payload["blocks"]!
            .Where(block => block["fields"] is not null)
            .SelectMany(block => block["fields"]!)
            .Select(field => field["text"]!.Value<string>())
            .ToList();
        Assert.That(fields, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_handler.RequestUri, Is.EqualTo(new Uri(TestWebhookUrl)));
            Assert.That(fields[0], Does.Contain("Compute Engine"));
            Assert.That(fields[1], Does.Contain("BigQuery"));
            Assert.That(payload["blocks"]![0]!["text"]!["text"]!.Value<string>(),
                Does.Contain(projectId).And.Contain("127"));
        }
    }

    private sealed class RecordingWebhookHandler : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }
        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            Body = await request.Content!.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
        }
    }
}
