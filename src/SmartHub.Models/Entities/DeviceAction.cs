using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartHub.Models.Entities
{
    public class DeviceAction
    {

        public string Id { get; set; }

        [Required, MaxLength(60)]
        public string DeviceId { get; set; }

        [MaxLength(100)]
        public string Component { get; set; }

        [MaxLength(100)]
        public string Capability { get; set; }

        [MaxLength(100)]
        public string Command { get; set; }

    }
}
