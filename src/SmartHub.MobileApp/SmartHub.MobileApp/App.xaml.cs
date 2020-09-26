﻿using SmartHub.MobileApp.Pages;
using SmartHub.MobileApp.Services;
using System;
using Xamarin.FluentInjector;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartHub.MobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            this.StartInjecting()

                .SetInitialPage<DevicesPage>()

                .AddHttpClient<RaspberryClient>(client => {
                    client.BaseAddress = new Uri("https://localhost:44396/");
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
