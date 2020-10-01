using Microsoft.AppCenter;
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

            AppCenter.Start(Config.AppCenter);

            CurrentProvider = this.StartInjecting()

                                    .SetInitialPage<DevicesPage>()

                                    .AddHttpClient<RaspberryClient>(client => {
                                        client.BaseAddress = new Uri(Config.Endpoint);
                                    })

                                    .Build();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
