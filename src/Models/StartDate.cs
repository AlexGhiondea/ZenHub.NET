using System;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class StartDate
    {
        [JsonPropertyName("start_date")]
        public DateTime Start { get; set; }
    }
}
