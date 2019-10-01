using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class EpicInfo
    {
        [JsonPropertyName("issue_number")]
        public int IssueNumber { get; set; }
   
        [JsonPropertyName("repo_id")]
        public long RepositoryId { get; set; }

        [JsonPropertyName("issue_url")]
        public string Url { get; set; }
    }
}