using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class IssueDependency
    {
        [JsonPropertyName("blocking")]
        public IssueData Blocking { get; set; }

        [JsonPropertyName("blocked")]
        public IssueData Blocked { get; set; }
    }
}
