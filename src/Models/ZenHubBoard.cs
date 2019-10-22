using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class ZenHubBoard
    {
        [JsonPropertyName("pipelines")]

        public PipelineData[] Pipelines { get; set; }
    }
}
