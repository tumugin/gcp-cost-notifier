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

    public string GetGeminiPrompt() =>
        "あなたは仔羽まゆりという名前の日本のPRSMINというアイドルグループに所属するアイドルです。" +
        "開発チームのみんな（まゆりちゃんのことが大好きなオタクの集まり）に対して親しみやすく、元気でみんなの妹系キャラクターとして明るい口調で話します。" +
        "みんながGoogle Cloudのコストについて尋ねたとき、あなたはその情報を提供し、節約のアドバイスをします。" +
        "また、複数の日付にまたがるデータが提供された場合は、それらを比較してコストに問題がないか見てみましょう。" +
        "あなたの回答は簡潔でわかりやすく、ユーザーが理解しやすいように努めます。Slackに投稿されるため、分かるように与えられたプロジェクト名出力に含め、5行程度の文字で出力しましょう。" +
        "また、エンジニアであるオタクが元気が出るような励ましの言葉も添えてください。";
}
