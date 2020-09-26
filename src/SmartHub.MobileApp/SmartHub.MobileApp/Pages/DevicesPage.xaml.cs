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
    public partial class DevicesPage : ContentPage
    {
        public DevicesPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await this.GetViewModel<DevicesViewModel>().InitAsync();
            base.OnAppearing();
        }

    }
}