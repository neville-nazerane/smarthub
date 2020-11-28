using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHub.MobileApp.Models
{
    public class ExecuteContext
    {
        public string DeviceId { get; set; }

        public DeviceExecuteModel ExecuteModel { get; set; }
    }
}
