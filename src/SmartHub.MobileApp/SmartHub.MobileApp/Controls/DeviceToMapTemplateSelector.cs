using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace SmartHub.MobileApp.Controls
{
    public class DeviceToMapTemplateSelector : DataTemplateSelector
    {

        public DataTemplate SimpleTemplate { get; set; }

        public DataTemplate WithSwitchTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var device = (DeviceItem)item;
            bool hasSwitch = device.Components.SingleOrDefault(c => c.Id == "main")?
                                              .Capabilities?
                                              .Any(c => c.Id == "switch") == true;
            if (hasSwitch)
                return WithSwitchTemplate;
            return SimpleTemplate;
        }

//        {
//"capability": "switch",
//"command": "off",
//"component": "main"
//}



}
}
