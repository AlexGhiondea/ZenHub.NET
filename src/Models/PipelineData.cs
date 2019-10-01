using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class PipelineData
    { 
        [JsonPropertyName("id")]
        public string Id { get; set;}

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("issues")]
        public IssueData[] Issues { get; set; }
    }
}
