namespace GCPCostNotifier;

using System.ComponentModel.DataAnnotations;

public class AppSetting
{
    [Required] public required string ProjectId { get; init; }

    [Required] public required string TargetTableName { get; init; }

    [Required] public required string SlackWebhookUrl { get; init; }

    public Character Character { get; init; } = Character.Mayuri;

    public string? BillingTargetProjectId { get; init; }
}
