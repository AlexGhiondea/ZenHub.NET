using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class EstimateValue
    {
        [JsonPropertyName("value")]
        public float Value { get; set; }
    }
}
