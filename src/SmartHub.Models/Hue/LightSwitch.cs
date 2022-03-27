using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SmartHub.Models.Hue
{
    public class LightSwitch
    {
        [JsonPropertyName("on")]
        public LightOnOff_OnModel On { get; set; }
    }

    public class LightOnOff_OnModel
    {
        [JsonPropertyName("on")]
        public bool On { get; set; }
    }


}
