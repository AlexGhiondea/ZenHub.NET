using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class IssueDependency
    {
        [JsonPropertyName("blocking")]
        public IssueDetails Blocking { get; set; }

        [JsonPropertyName("blocked")]
        public IssueDetails Blocked { get; set; }
    }
}
