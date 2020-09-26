using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SmartHub.MobileApp.Utils
{
    public static class ViewModelExtensions
    {

        public static TViewModel GetViewModel<TViewModel>(this Page page) 
            where TViewModel : class
            => page.BindingContext as TViewModel;

    }
}
