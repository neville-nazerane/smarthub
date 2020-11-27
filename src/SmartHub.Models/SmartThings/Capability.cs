using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SmartHub.Models.SmartThings
{

    public class CapabilityData
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        //public CapabilityAttributes Attributes { get; set; }
        public Dictionary<string, object> Commands { get; set; }

        public IEnumerable<string> GetCommands() => Commands.Keys;

    }

    public class CapabilityAttributes
    {
        public CapabilitySwitch Switch { get; set; }
    }

    public class CapabilitySwitch
    {
        public CapabilitySchema Schema { get; set; }
        public CapabilityEnumcommand[] EnumCommands { get; set; }
    }

    public class CapabilitySchema
    {
        public string Type { get; set; }
        public CapabilityProperties Properties { get; set; }
        public bool AdditionalProperties { get; set; }
        public string[] Required { get; set; }
    }

    public class CapabilityProperties
    {
        public CapabilityValue Value { get; set; }
    }

    public class CapabilityValue
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string[] Enum { get; set; }
    }

    public class CapabilityEnumcommand
    {
        public string Command { get; set; }
        public string Value { get; set; }
    }


}
