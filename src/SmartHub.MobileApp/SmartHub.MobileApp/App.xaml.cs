using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using SmartHub.MobileApp.Pages;
using SmartHub.MobileApp.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
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

            ExperimentalFeatures.Enable(new string[] {
                "SwipeView_Experimental",
                "Shapes_Experimental"
            });


            InitializeComponent();

            AppCenter.Start(Config.AppCenter,
                                typeof(Analytics), typeof(Crashes));

            CurrentProvider = this.StartInjecting()

                                    .SetInitialPage<HomePage>()

                                    .AddHttpClient<RaspberryClient>(client => {
                                        client.BaseAddress = new Uri(Config.Endpoint);
                                    })

                                    .Build();
        }

    }
}
