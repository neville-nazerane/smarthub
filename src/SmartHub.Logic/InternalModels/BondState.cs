using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartHub.Logic.InternalModels
{
    public class BondState
    {

        [JsonPropertyName("power")]
        public int Power { get; set; }

        [JsonPropertyName("speed")]
        public int Speed { get; set; }

        [JsonPropertyName("light")]
        public int Light { get; set; }
    }

}
