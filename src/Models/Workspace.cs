using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class Workspace
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("repositories")]
        public long[] Repositories { get; set; }
    }
}
