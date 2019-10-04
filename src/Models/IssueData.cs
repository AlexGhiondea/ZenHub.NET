using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class IssueData
    {
        [JsonPropertyName("issue_number")]
        public int IssueNumber { get; set; }

        [JsonPropertyName("is_epic")]
        public bool IsEpic { get; set; }

        [JsonPropertyName("repo_id")]
        public long RepositoryId { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("estimate")]
        public EstimateValue Estimate { get; set; }

        [JsonPropertyName("pipelines")]
        public Pipeline[] Pipelines { get; set; }

        [JsonPropertyName("pipeline")]
        public Pipeline Pipeline { get; set; }

        [JsonPropertyName("plus_ones")]
        public CreatedDate[] PlusOnes { get; set; }
    }
}
