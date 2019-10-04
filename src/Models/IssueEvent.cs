using System;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class IssueEvent
    {
        [JsonPropertyName("user_id")]
        public long UserId { get; set; }

        [JsonPropertyName("type")]
        public string EventType { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("from_estimate")]
        public EstimateValue FromEstimate { get; set; }

        [JsonPropertyName("to_estimate")]
        public EstimateValue ToEstimate { get; set; }

        [JsonPropertyName("to_pipeline")]
        public Pipeline ToPipeline { get; set; }

        [JsonPropertyName("from_pipeline")]
        public Pipeline FromPipeline { get; set; }

        [JsonPropertyName("workspace_id")]
        public string WorkspaceId { get; set; }
    }
}
