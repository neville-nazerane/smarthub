using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
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
        private bool _isActionLoading;
        private ObservableCollection<ActionInfo> _actions;
        private readonly RaspberryClient _client;
        private readonly IPageControl _pageControl;

        public bool IsActionLoading { get => _isActionLoading; set => SetProperty(ref _isActionLoading, value); }

        public ObservableCollection<ActionInfo> Actions { get => _actions; set => SetProperty(ref _actions, value); }

        public HomeViewModel(RaspberryClient client, IPageControl pageControl)
        {
            _client = client;
            _pageControl = pageControl;

            IsActionLoading = true;
            SetActionsAsync().ContinueWith(o => Task.FromResult(IsActionLoading = false));
        }

        private async Task SetActionsAsync()
        {
            try
            {
                Actions = new ObservableCollection<ActionInfo>(await _client.GetActionsAsync());
            }
            catch (Exception e)
            {
                await _pageControl.DisplayAlert("Error", "Failed fetch actions", "Ok");
                Crashes.TrackError(e);
            }
        }

    }
}
