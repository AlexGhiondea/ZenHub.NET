using Octokit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class EpicData
    {
        [JsonPropertyName("total_epic_estimates")]
        public EstimateValue TotalEstimate { get; set; }

        [JsonPropertyName("estimate")]
        public EstimateValue Estimate { get; set; }

        [JsonPropertyName("pipeline")]
        public Pipeline Pipeline { get; set; }

        [JsonPropertyName("pipelines")]
        public Pipeline[] Pipelines { get; set; }

        [JsonPropertyName("issues")]
        public IssueData[] Issues { get; set; }
    }
}
