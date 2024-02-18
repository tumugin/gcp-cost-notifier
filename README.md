[![tests](https://github.com/tumugin/gcp-cost-notifier/actions/workflows/test.yaml/badge.svg?branch=main)](https://github.com/tumugin/gcp-cost-notifier/actions/workflows/test.yaml)

# gcp-cost-notifier

GCPの前日分の確定された利用料金をSlackにお知らせしてくれるCloud Functionsです。

# 設定(環境変数)

- `AppSettings__ProjectId` : GCPのプロジェクトID
- `AppSettings__SlackWebhookUrl` : 送信先のSlackのWebhook URL
- `AppSettings__TargetTableName` : 抽出元のBigQueryのテーブル名
- `DOTNET_ENVIRONMENT` : デバッグでない限り`Production`を指定
- `DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP3SUPPORT` : (WORKAROUND) `false`を指定。Cloud Functionsでは上手くHTTP/3が動作しないため。
  - https://github.com/dotnet/runtime/issues/94794 を参照

# Terraform
日本時間の0時に自動的にFunctionsを実行する例。sourceについてはCSRを使用するか、GCSにアップロードしたZIPファイルを指定してください。

```hcl
resource "google_cloudfunctions2_function" "cost_notification_function" {
  name        = "cost-notification-function"
  location    = "asia-northeast1"
  description = "仔羽まゆりちゃんがGCPのコストを通知する関数"

  build_config {
    runtime     = "dotnet8"
    entry_point = "GCPCostNotifier.YesterdayCostNotifyFunction"
    source {
      repo_source {
        repo_name  = "github_tumugin_gcp-cost-notifier"
        commit_sha = "92666cc296a968ce180341b00ad89ee78920b017"
      }
    }
  }

  service_config {
    max_instance_count               = 1
    min_instance_count               = 0
    available_memory                 = "128Mi"
    timeout_seconds                  = 120
    max_instance_request_concurrency = 1
    available_cpu                    = "0.083"
    environment_variables = {
      // workaround: https://github.com/dotnet/runtime/issues/94794
      DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP3SUPPORT = "false"
      DOTNET_ENVIRONMENT                                     = "Production"
      AppSettings__ProjectId                                 = var.project
      AppSettings__SlackWebhookUrl                           = "*******"
      AppSettings__TargetTableName                           = "*******"
    }
    ingress_settings               = "ALLOW_INTERNAL_ONLY"
    all_traffic_on_latest_revision = true
  }

  event_trigger {
    trigger_region = "asia-northeast1"
    event_type     = "google.cloud.pubsub.topic.v1.messagePublished"
    pubsub_topic   = google_pubsub_topic.cost_notification_trigger_topic.id
    retry_policy   = "RETRY_POLICY_RETRY"
  }
}

resource "google_pubsub_topic" "cost_notification_trigger_topic" {
  name = "cost-notification-trigger-topic"
}

resource "google_cloud_scheduler_job" "invoke_cost_notification_function" {
  name        = "invoke-cost-notification-function"
  description = "コスト通知用のFunctionを定期実行するジョブ"
  schedule    = "0 0 * * *"
  region      = "asia-northeast1"
  time_zone   = "Asia/Tokyo"

  pubsub_target {
    topic_name = google_pubsub_topic.cost_notification_trigger_topic.id
    data       = base64encode("invoke")
  }
}
```
