﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ZenHub.Models
{
    public class CreatedData
    {
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
