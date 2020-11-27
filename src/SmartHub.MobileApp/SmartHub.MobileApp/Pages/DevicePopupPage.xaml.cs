using SmartHub.MobileApp.Utils;
using SmartHub.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartHub.MobileApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicePopupPage : ContentPage
    {
        public DevicePopupPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            this.GetViewModel<DevicePopupViewModel>().Cancel();
            return base.OnBackButtonPressed();
        }
    }
}