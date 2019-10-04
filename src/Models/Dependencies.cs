using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class RepoDependencies
    {
        [JsonPropertyName("dependencies")]
        public IssueDependency[] Dependencies { get; set; }
    }
}
