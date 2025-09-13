namespace GCPCostNotifier.Services;

public interface ICharacterService
{
    public string GetGreetingMessage(decimal totalCost);
    public string GetAttachmentText();
    public string GetFooterText();
    public string GetColor();
}
