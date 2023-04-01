using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SmartHub.Models.HueSync
{
    public class ExecutionRequest
    {

        [JsonPropertyName("syncActive")]
        public bool SyncActive { get; set; }

    }
}
