[![tests](https://github.com/tumugin/gcp-cost-notifier/actions/workflows/test.yaml/badge.svg?branch=main)](https://github.com/tumugin/gcp-cost-notifier/actions/workflows/test.yaml)

# gcp-cost-notifier

GCPの前日分の確定された利用料金をSlackにお知らせしてくれるCloud Functionsです。

## 要件

- .NET 10.0 以上

## キャラクター機能

このアプリケーションは2つのキャラクターから選択できます：

- **まゆりちゃん** (mayuri) : 仔羽まゆりをイメージしたキャラクター。かわいらしい口調でお知らせします。
- **ちあちゃん** (chia) : よりクールな口調でお知らせするキャラクターです。

キャラクターによってメッセージの文言や色が変わります。

## 設定(環境変数)

### 必須設定

- `AppSettings__ProjectId` : GCPのプロジェクトID
- `AppSettings__SlackWebhookUrl` : 送信先のSlackのWebhook URL
- `AppSettings__TargetTableName` : 抽出元のBigQueryのテーブル名

### オプション設定

- `AppSettings__Character` : 使用するキャラクター（"Mayuri"または"Chia"、省略時は"Mayuri"）
- `AppSettings__BillingTargetProjectId` : 課金対象のプロジェクトIDを指定する場合に設定
- `AppSettings__UseGeminiOutput` : Geminiによる出力を使用する場合は`true`を設定（デフォルト: `false`）
- `AppSettings__GeminiApiKey` : Gemini APIキー（`UseGeminiOutput`が`true`の場合に必要）
- `AppSettings__GeminiModelName` : 使用するGeminiモデル名（デフォルト: `gemini-2.5-flash`）
- `DOTNET_ENVIRONMENT` : デバッグでない限り`Production`を指定
- `DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP3SUPPORT` : (WORKAROUND) `false`を指定。Cloud Functionsでは上手くHTTP/3が動作しないため。
  - https://github.com/dotnet/runtime/issues/94794 を参照

## ローカルでのデバッグ方法

### Functions Framework for .NETを使用

```bash
# プロジェクトディレクトリに移動
cd GCPCostNotifier

# 環境変数を設定
export DOTNET_ENVIRONMENT=Development
export AppSettings__ProjectId="your-gcp-project-id"
export AppSettings__SlackWebhookUrl="your-slack-webhook-url"
export AppSettings__TargetTableName="your-bigquery-table-name"
export AppSettings__Character="Mayuri"  # または "Chia"

# Functions Frameworkで実行
dotnet run
```

### 設定ファイルを使用する場合

`GCPCostNotifier/appsettings.Development.json`を作成：

```json
{
  "AppSettings": {
    "ProjectId": "your-gcp-project-id",
    "SlackWebhookUrl": "your-slack-webhook-url",
    "TargetTableName": "your-bigquery-table-name",
    "Character": "Mayuri",
    "UseGeminiOutput": false,
    "GeminiApiKey": "your-gemini-api-key",
    "GeminiModelName": "gemini-2.5-flash"
  }
}
```

### Cloud Eventのテスト

Functions Frameworkが起動後（デフォルトで`http://localhost:8080`）、以下のようにHTTP POSTでCloud Eventを送信：

```bash
curl -X POST http://localhost:8080 \
  -H "Content-Type: application/json" \
  -H "ce-specversion: 1.0" \
  -H "ce-type: google.cloud.pubsub.topic.v1.messagePublished" \
  -H "ce-source: //pubsub.googleapis.com/projects/test/topics/test" \
  -H "ce-id: test-id" \
  -d '{"message": {"data": "aW52b2tl"}}'
```

## Terraform
日本時間の0時に自動的にCloud Runサービスを実行する例。GitHub Container Registryの公開イメージを使用します。

```hcl
resource "google_artifact_registry_repository" "ghcr" {
  location      = "asia-northeast1"
  repository_id = "ghcr"
  description   = "Remote docker repository for GitHub Container Registry"
  format        = "DOCKER"
  mode          = "REMOTE_REPOSITORY"

  remote_repository_config {
    description                 = "GitHub Container Registry"
    disable_upstream_validation = true
    docker_repository {
      custom_repository {
        uri = "https://ghcr.io"
      }
    }
  }
}

resource "google_cloud_run_v2_service" "cost_notification_service" {
  name     = "cost-notification-service"
  location = "asia-northeast1"
  ingress  = "INGRESS_TRAFFIC_INTERNAL_ONLY"

  template {
    containers {
      // Image of GitHub Container Registry(See releases for newest tag)
      image = "${google_artifact_registry_repository.ghcr.location}-docker.pkg.dev/${var.project}/${google_artifact_registry_repository.ghcr.name}/tumugin/gcp-cost-notifier:6c17e0d60ebd9ca5a3d44157dd11a8446b389ee0"

      // workaround: https://github.com/dotnet/runtime/issues/94794
      env {
        name  = "DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP3SUPPORT"
        value = "false"
      }
      env {
        name  = "DOTNET_ENVIRONMENT"
        value = "Production"
      }
      env {
        name  = "AppSettings__ProjectId"
        value = var.project
      }
      env {
        name  = "AppSettings__SlackWebhookUrl"
        // Set your Slack Webhook URL
        value = "https://hooks.slack.com/services/*******"
      }
      env {
        name  = "AppSettings__TargetTableName"
        // Set your BigQuery Billing Export Table name
        value = "project-name.dataset-name.gcp_billing_export_v1_*******"
      }
      env {
        name  = "AppSettings__Character"
        // Set character "Mayuri" or "Chia" (default: "Mayuri")
        value = "Mayuri"
      }
      // Optional: Enable Gemini output
      // env {
      //   name  = "AppSettings__UseGeminiOutput"
      //   value = "true"
      // }
      // env {
      //   name  = "AppSettings__GeminiApiKey"
      //   value = "your-gemini-api-key"
      // }
      // env {
      //   name  = "AppSettings__GeminiModelName"
      //   value = "gemini-2.5-flash"
      // }

      resources {
        limits = {
          cpu    = "1"
          memory = "256Mi"
        }
        cpu_idle = true
      }
    }

    scaling {
      min_instance_count = 0
      max_instance_count = 1
    }

    timeout = "120s"
  }
}

resource "google_service_account" "cloud_run_invoker" {
  account_id   = "cloud-run-invoker"
  display_name = "Cloud Run Invoker Service Account"
}

resource "google_cloud_run_v2_service_iam_member" "invoker" {
  location = google_cloud_run_v2_service.cost_notification_service.location
  name     = google_cloud_run_v2_service.cost_notification_service.name
  role     = "roles/run.invoker"
  member   = "serviceAccount:${google_service_account.cloud_run_invoker.email}"
}

resource "google_pubsub_subscription" "cost_notification_subscription" {
  name  = "cost-notification-subscription"
  topic = google_pubsub_topic.cost_notification_trigger_topic.name

  push_config {
    push_endpoint = google_cloud_run_v2_service.cost_notification_service.uri

    oidc_token {
      service_account_email = google_service_account.cloud_run_invoker.email
    }
  }
}

resource "google_pubsub_topic" "cost_notification_trigger_topic" {
  name = "cost-notification-trigger-topic"
}

resource "google_cloud_scheduler_job" "invoke_cost_notification_service" {
  name        = "invoke-cost-notification-service"
  description = "コスト通知用のCloud Runサービスを定期実行するジョブ"
  schedule    = "0 0 * * *"
  region      = "asia-northeast1"
  time_zone   = "Asia/Tokyo"

  pubsub_target {
    topic_name = google_pubsub_topic.cost_notification_trigger_topic.id
    data       = base64encode("invoke")
  }
}
```
