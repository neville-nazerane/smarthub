using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHub.MobileApp.Controls
{
    public class ExpandableData
    {

        public string Text { get; set; }

        public CollectionLoadControl LoadControl { get; set; }

        public ExpandableData()
        {
            LoadControl = new CollectionLoadControl();
        }

    }
}
