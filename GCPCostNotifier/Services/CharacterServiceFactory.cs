namespace GCPCostNotifier.Services;

public static class CharacterServiceFactory
{
    public static ICharacterService Create(Character character) =>
        character switch
        {
            Character.Chia => new ChiaCharacterService(),
            Character.Mayuri => new MayuriCharacterService(),
            _ => throw new ArgumentOutOfRangeException(nameof(character))
        };
}
