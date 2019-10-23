using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class EpicDetails
    {
        [JsonPropertyName("total_epic_estimates")]
        public EstimateValue TotalEstimate { get; set; }

        [JsonPropertyName("estimate")]
        public EstimateValue Estimate { get; set; }

        [JsonPropertyName("pipeline")]
        public ZenHubPipeline Pipeline { get; set; }

        [JsonPropertyName("pipelines")]
        public ZenHubPipeline[] Pipelines { get; set; }

        [JsonPropertyName("issues")]
        public IssueDetails[] Issues { get; set; }
    }
}
