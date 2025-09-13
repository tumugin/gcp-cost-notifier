namespace GCPCostNotifier.Services;

public class ChiaCharacterService : ICharacterService
{
    public string GetGreetingMessage(decimal totalCost) =>
        "*:purple_heart: ちあちゃんが昨日のGCPのコストをお知らせします :purple_heart:*\n" +
        $"昨日のGCPのコストは *{totalCost.ToJpyStyleString()}* ですね～\n" +
        "お金の管理は大切ですよ！";

    public string GetAttachmentText() => "昨日のGCPのコストの詳細です（1円未満のものは省略しました）";

    public string GetFooterText() => "コストを計算するちあちゃん";

    public string GetColor() => "#C8A2C8";
}
