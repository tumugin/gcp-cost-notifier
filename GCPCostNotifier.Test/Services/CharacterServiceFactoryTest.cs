using GCPCostNotifier.Services;

namespace GCPCostNotifier.Test.Services;

[TestFixture]
public class CharacterServiceFactoryTest
{
    [Test]
    public void CreateMayuriCharacterReturnsCorrectService()
    {
        var service = CharacterServiceFactory.Create(Character.Mayuri);

        Assert.That(service, Is.InstanceOf<MayuriCharacterService>());
        Assert.That(service.GetColor(), Is.EqualTo("#ADE0EE"));
    }

    [Test]
    public void CreateChiaCharacterReturnsCorrectService()
    {
        var service = CharacterServiceFactory.Create(Character.Chia);

        Assert.That(service, Is.InstanceOf<ChiaCharacterService>());
        Assert.That(service.GetColor(), Is.EqualTo("#FFFFFF"));
    }

    [Test]
    public void CreateUniCharacterReturnsCorrectService()
    {
        var service = CharacterServiceFactory.Create(Character.Uni);

        Assert.That(service, Is.InstanceOf<UniCharacterService>());
        Assert.That(service.GetColor(), Is.EqualTo("#F5F0EB"));
    }
}
