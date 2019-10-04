using Octokit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class RepoDependencies
    {
        [JsonPropertyName("dependencies")]
        public IssueDependency[] Dependencies { get; set; }
    }
}
