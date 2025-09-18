using GCPCostNotifier.Services;

namespace GCPCostNotifier.Test.Services;

[TestFixture]
public class CharacterServiceTest
{
    [Test]
    public void MayuriCharacterServiceGetGreetingMessageReturnsCorrectMessage()
    {
        var service = new MayuriCharacterService();
        var totalCost = 1500m;
        var projectId = "test-project";
        var result = service.GetGreetingMessage(totalCost, projectId);

        Assert.That(result, Does.Contain("まゆりちゃん"));
        Assert.That(result, Does.Contain("￥1,500"));
        Assert.That(result, Does.Contain("お金使いすぎないで欲しいな"));
        Assert.That(result, Does.Contain(":white_heart:"));
        Assert.That(result, Does.Contain("test-project"));
    }

    [Test]
    public void MayuriCharacterServiceGetAttachmentTextReturnsCorrectText()
    {
        var service = new MayuriCharacterService();
        var result = service.GetAttachmentText();

        Assert.That(result, Is.EqualTo("昨日のGoogle Cloudのコストの詳細だよ！（1円未満のものは省略したよ！）"));
    }

    [Test]
    public void MayuriCharacterServiceGetFooterTextReturnsCorrectText()
    {
        var service = new MayuriCharacterService();
        var result = service.GetFooterText();

        Assert.That(result, Is.EqualTo("コストを計算する仔羽まゆりちゃん"));
    }

    [Test]
    public void MayuriCharacterServiceGetColorReturnsCorrectColor()
    {
        var service = new MayuriCharacterService();
        var result = service.GetColor();

        Assert.That(result, Is.EqualTo("#ADE0EE"));
    }

    [Test]
    public void ChiaCharacterServiceGetGreetingMessageReturnsCorrectMessage()
    {
        var service = new ChiaCharacterService();
        var totalCost = 2000m;
        var projectId = "chia-test-project";
        var result = service.GetGreetingMessage(totalCost, projectId);

        Assert.That(result, Does.Contain("ちあちゃん"));
        Assert.That(result, Does.Contain("￥2,000"));
        Assert.That(result, Does.Contain("お金の使い方気をつけようね"));
        Assert.That(result, Does.Contain(":hamster:"));
        Assert.That(result, Does.Contain("chia-test-project"));
    }

    [Test]
    public void ChiaCharacterServiceGetAttachmentTextReturnsCorrectText()
    {
        var service = new ChiaCharacterService();
        var result = service.GetAttachmentText();

        Assert.That(result, Is.EqualTo("昨日のGoogle Cloudのコストの詳細だよ〜♪（1円未満のものは省略しちゃった！）"));
    }

    [Test]
    public void ChiaCharacterServiceGetFooterTextReturnsCorrectText()
    {
        var service = new ChiaCharacterService();
        var result = service.GetFooterText();

        Assert.That(result, Is.EqualTo("コストを計算する未白ちあ"));
    }

    [Test]
    public void ChiaCharacterServiceGetColorReturnsCorrectColor()
    {
        var service = new ChiaCharacterService();
        var result = service.GetColor();

        Assert.That(result, Is.EqualTo("#FFFFFF"));
    }
}
