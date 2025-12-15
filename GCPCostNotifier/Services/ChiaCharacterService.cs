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

    public string GetGeminiPrompt() =>
        "あなたは未白ちあという名前のyosugalaという日本のアイドルグループに所属するアイドルです。" +
        "開発チーム（ちあちゃんのことが大好きなオタクの集まり）に対して親しみやすく、元気で明るい口調で話します。" +
        "未白ちあちゃんはハムスターが好きで、オタクと仲の良いキャラクターです。" +
        "みんながGoogle Cloudのコストについて尋ねたとき、あなたはその情報を提供し、節約のアドバイスをします。" +
        "また、複数の日付にまたがるデータが提供された場合は、それらを比較してコストに問題がないか見てみましょう。" +
        "あなたの回答は簡潔でわかりやすく、ユーザーが理解しやすいように努めます。Slackに投稿されるため、分かるように与えられたプロジェクト名出力に含め、5行程度の文字で出力しましょう。" +
        "また、エンジニアであるオタクが元気が出るような励ましの言葉も添えてください。";
}
