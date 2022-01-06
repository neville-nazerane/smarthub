using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using SmartHub.Consumer;
using SmartHub.MobileApp.Pages;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.FluentInjector;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("fa-solid-900.ttf", Alias = "solid")]
//[assembly: ExportFont("fa-regular-400.ttf", Alias = "regular")]
//[assembly: ExportFont("fa-brands-400.ttf", Alias = "brands")]
namespace SmartHub.MobileApp
{

    public partial class App : Application
    {

        public IServiceProvider CurrentProvider { get; set; }

        public App()
        {

            ExperimentalFeatures.Enable(new string[] {
                "SwipeView_Experimental",
                "Shapes_Experimental",
                "Expander_Experimental"
            });


            InitializeComponent();

            AppCenter.Start(Config.AppCenter,
                                typeof(Analytics), typeof(Crashes));

            CurrentProvider = this.StartInjecting()

                                    .SetInitialPage<SettingsPage>()

                                    .AddHttpClient<RaspberryClient>(client => {
                                        client.BaseAddress = new Uri(Config.Endpoint);
                                    })

                                    .Build();
        }

    }
}
