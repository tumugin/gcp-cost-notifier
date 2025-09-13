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
        var result = service.GetGreetingMessage(totalCost);

        Assert.That(result, Does.Contain("まゆりちゃん"));
        Assert.That(result, Does.Contain("￥1,500"));
        Assert.That(result, Does.Contain("お金使いすぎないで欲しいな"));
        Assert.That(result, Does.Contain(":white_heart:"));
    }

    [Test]
    public void MayuriCharacterServiceGetAttachmentTextReturnsCorrectText()
    {
        var service = new MayuriCharacterService();
        var result = service.GetAttachmentText();

        Assert.That(result, Is.EqualTo("昨日のGCPのコストの詳細だよ！（1円未満のものは省略したよ！）"));
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
        var result = service.GetGreetingMessage(totalCost);

        Assert.That(result, Does.Contain("ちあちゃん"));
        Assert.That(result, Does.Contain("￥2,000"));
        Assert.That(result, Does.Contain("お金の管理は大切ですよ"));
        Assert.That(result, Does.Contain(":purple_heart:"));
    }

    [Test]
    public void ChiaCharacterServiceGetAttachmentTextReturnsCorrectText()
    {
        var service = new ChiaCharacterService();
        var result = service.GetAttachmentText();

        Assert.That(result, Is.EqualTo("昨日のGCPのコストの詳細です（1円未満のものは省略しました）"));
    }

    [Test]
    public void ChiaCharacterServiceGetFooterTextReturnsCorrectText()
    {
        var service = new ChiaCharacterService();
        var result = service.GetFooterText();

        Assert.That(result, Is.EqualTo("コストを計算するちあちゃん"));
    }

    [Test]
    public void ChiaCharacterServiceGetColorReturnsCorrectColor()
    {
        var service = new ChiaCharacterService();
        var result = service.GetColor();

        Assert.That(result, Is.EqualTo("#C8A2C8"));
    }
}
