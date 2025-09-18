namespace GCPCostNotifier.Services;

public class MayuriCharacterService : ICharacterService
{
    public string GetGreetingMessage(decimal totalCost, string projectId) =>
        "*:white_heart: まゆりちゃんが昨日のGoogle Cloudのコストをお知らせします :white_heart:*\n" +
        $"昨日のGoogle Cloudのコストは *{totalCost.ToJpyStyleString()}* だよ！！\n" +
        "お金使いすぎないで欲しいな！\n" +
        $"(プロジェクト: {projectId})";

    public string GetAttachmentText() => "昨日のGoogle Cloudのコストの詳細だよ！（1円未満のものは省略したよ！）";

    public string GetFooterText() => "コストを計算する仔羽まゆりちゃん";

    public string GetColor() => "#ADE0EE";
}
