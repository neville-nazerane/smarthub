using SmartHub.MobileApp.Components;
using SmartHub.MobileApp.Controls;
using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ValueConverters
{
    public class ToExpandable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case DeviceItem deviceItem:
                    return new ExpandableData { 
                        Text = deviceItem.Label
                    };
                default:
                    throw new InvalidCastException($"Can't convert {value.GetType()} to Expandable");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
