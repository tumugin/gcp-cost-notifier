namespace GCPCostNotifier.Services;

public class ChiaCharacterService : ICharacterService
{
    public string GetGreetingMessage(decimal totalCost, string projectId) =>
        "*:white_heart: 未白ちあが昨日のGCPのコストをお知らせするよ〜 :white_heart:*\n" +
        $"昨日のGCPのコストは *{totalCost.ToJpyStyleString()}* だったよ！\n" +
        "みんな〜、お金の使い方気をつけようね♪\n" +
        $"(プロジェクト: {projectId})";

    public string GetAttachmentText() => "昨日のGCPのコストの詳細だよ〜♪（1円未満のものは省略しちゃった！）";

    public string GetFooterText() => "コストを計算する未白ちあ";

    public string GetColor() => "#FFFFFF";
}
