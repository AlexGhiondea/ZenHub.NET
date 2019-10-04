using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class EpicList
    {
        [JsonPropertyName("epic_issues")]
        public EpicInfo[] Epics { get; set; }
    }
}