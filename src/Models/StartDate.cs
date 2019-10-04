using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class StartDate
    {
        [JsonPropertyName("start_date")]
        public DateTime Start { get; set; }
    }
}
