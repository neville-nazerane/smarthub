using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SmartHub.MobileApp.TemplateSelectors
{
    public class ExpandableTemplateSelector : DataTemplateSelector
    {

        public DataTemplate ExpandableTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            throw new NotImplementedException();
        }
    }
}
