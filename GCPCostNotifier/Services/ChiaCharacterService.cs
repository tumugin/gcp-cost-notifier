namespace GCPCostNotifier.Services;

public class ChiaCharacterService : ICharacterService
{
    public string GetGreetingMessage(decimal totalCost, string projectId) =>
        "*:hamster: ちあちゃんが昨日のGoogle Cloudのコストをお知らせするよ〜 :hamster:*\n" +
        $"昨日のGoogle Cloudのコストは *{totalCost.ToJpyStyleString()}* だったよ！\n" +
        "みんな〜、お金の使い方気をつけようね♪\n" +
        $"(プロジェクト: {projectId})";

    public string GetAttachmentText() => "昨日のGoogle Cloudのコストの詳細だよ〜♪（1円未満のものは省略しちゃった！）";

    public string GetFooterText() => "コストを計算する未白ちあ";

    public string GetColor() => "#FFFFFF";
}
