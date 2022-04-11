using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SmartHub.Models.Hue
{

    public class HueEvent
    {

        //[JsonPropertyName("creationtime")]
        //public DateTime Creationtime { get; set; }

        [JsonPropertyName("data")]
        public IEnumerable<HueEventData> Data { get; set; }

        //[JsonPropertyName("id")]
        //public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

    }

    public class HueEventData
    {

        [JsonPropertyName("id")]
        public string Id { get; set; }

        //[JsonPropertyName("id_v1")]
        //public string Id_v1 { get; set; }

        //[JsonPropertyName("owner")]
        //public Owner Owner { get; set; }

        [JsonPropertyName("motion")]
        public HueMotion Motion { get; set; }

        [JsonPropertyName("button")]
        public HueButton Button { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class HueButton
    {
        [JsonPropertyName("last_event")]
        public string LastEvent { get; set; }

    }

    public class HueMotion
    {
        [JsonPropertyName("motion_valid")]
        public bool MotionValid { get; set; }

        [JsonPropertyName("motion")]
        public bool Motion { get; set; }
    }

    //public class Owner
    //{
    //    public string rid { get; set; }
    //    public string rtype { get; set; }
    //}

    //public class Temperature
    //{
    //    public float temperature { get; set; }
    //    public bool temperature_valid { get; set; }
    //}

}
