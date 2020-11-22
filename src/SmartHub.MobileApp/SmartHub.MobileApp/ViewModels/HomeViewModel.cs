using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using SmartHub.MobileApp.Controls;
using SmartHub.MobileApp.Services;
using SmartHub.Models.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.FluentInjector.Utilities;

namespace SmartHub.MobileApp.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private ObservableCollection<ActionInfo> _actions;
        private readonly RaspberryClient _client;
        private readonly IPageControl _pageControl;

        public LoadControl ActionsLoading { get; set; }
        public ObservableCollection<ActionInfo> Actions { get => _actions; set => SetProperty(ref _actions, value); }

        public HomeViewModel(RaspberryClient client, IPageControl pageControl)
        {
            _client = client;
            _pageControl = pageControl;

            ActionsLoading = new LoadControl
            {
                OnFailAsync = ActionsLoadFailed
            };
            ActionsLoading.Execute(LoadActionsAsync);
        }

        async Task LoadActionsAsync()
        {
            Actions = new ObservableCollection<ActionInfo>(await _client.GetActionsAsync());
            await Task.Delay(3000);
        }

        private async Task ActionsLoadFailed(Exception e)
        {
            await _pageControl.DisplayAlert("Error", "Failed fetch actions", "Ok");
            Crashes.TrackError(e);
        }
    }
}
