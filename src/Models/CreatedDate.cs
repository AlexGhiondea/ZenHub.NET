using System;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class CreatedDate
    {
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
