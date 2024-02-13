namespace GCPCostNotifier.Services;

public class CalculatedDateTimeOffsets
{
    /// <summary>
    /// 計算に用いる基準日
    /// </summary>
    public required DateTimeOffset ReferenceDateTimeOffset { get; init; }

    /// <summary>
    /// 基準日から算出した課金区間の開始日時
    /// </summary>
    public required DateTimeOffset StartOffsetDateTimeOffset { get; init; }

    /// <summary>
    /// 基準日から算出した課金区間の終了日時
    /// </summary>
    public required DateTimeOffset EndOffsetDateTimeOffset { get; init; }

    /// <summary>
    /// 基準日から算出した課金区間の開始日時のパーティション(課金情報のテーブルの絞り込みに使用する)
    /// </summary>
    public required DateTimeOffset PartitionStartDateTimeOffset { get; init; }

    /// <summary>
    /// 基準日から算出した課金区間の終了日時のパーティション(課金情報のテーブルの絞り込みに使用する)
    /// </summary>
    public required DateTimeOffset PartitionEndDateTimeOffset { get; init; }
}
