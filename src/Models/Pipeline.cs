using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class Pipeline
    {
        [JsonPropertyName("workspace_id")]
        public string WorkspaceId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pipeline_id")]
        public string PipelineId { get; set; }
    }
}
