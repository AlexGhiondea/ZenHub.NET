using System;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class ReleaseReport
    {
        [JsonPropertyName("release_id")]
        public string ReleaseId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("desired_end_date")]
        public DateTime? DesiredEndDate { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("closed_at")]
        public DateTime? ClosedAt { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("repositories")]
        public long[] Repositories { get; set; }
    }
}
