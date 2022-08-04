using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartHub.Logic.InternalModels
{
    public class BondArgument
    {

        [JsonPropertyName("argument")]
        public int Argument { get; set; }

    }
}
