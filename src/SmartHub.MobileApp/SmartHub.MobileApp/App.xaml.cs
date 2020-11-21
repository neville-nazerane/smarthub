using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using SmartHub.MobileApp.Pages;
using SmartHub.MobileApp.Services;
using System;
using System.Threading.Tasks;
using Xamarin.FluentInjector;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartHub.MobileApp
{
    public partial class App : Application
    {

        public IServiceProvider CurrentProvider { get; set; }

        public App()
        {
            InitializeComponent();

            AppCenter.Start(Config.AppCenter,
                                typeof(Analytics), typeof(Crashes));

            CurrentProvider = this.StartInjecting()

                                    .SetInitialPage<DevicesPage>()

                                    .AddHttpClient<RaspberryClient>(client => {
                                        client.BaseAddress = new Uri(Config.Endpoint);
                                    })

                                    .Build();
        }

    }
}
