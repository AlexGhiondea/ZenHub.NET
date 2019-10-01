using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class EstimateValue
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
