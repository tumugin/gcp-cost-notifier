namespace GCPCostNotifier;

using System.ComponentModel.DataAnnotations;

public class AppSetting
{
    [Required] public string ProjectId { get; private set; } = null!;

    [Required] public string TargetTableName { get; private set; } = null!;

    [Required] public string SlackWebhookUrl { get; private set; } = null!;
}
