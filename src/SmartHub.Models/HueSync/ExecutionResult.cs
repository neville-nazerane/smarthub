using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SmartHub.Models.HueSync
{
    public class ExecutionResult
    {

        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("syncActive")]
        public bool SyncActive { get; set; }

        [JsonPropertyName("hdmiActive")]
        public bool HdmiActive { get; set; }

        [JsonPropertyName("hdmiSource")]
        public string HdmiSource { get; set; }

    }
}
