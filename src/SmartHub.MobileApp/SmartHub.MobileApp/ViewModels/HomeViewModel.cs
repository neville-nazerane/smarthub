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
using System.Windows.Input;
using Xamarin.FluentInjector.Utilities;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private ObservableCollection<ActionInfo> _actions;
        private bool _isRefreshing;
        private readonly RaspberryClient _client;
        private readonly IPageControl _pageControl;
        private readonly IInjectionControl _injectionControl;

        public LoadControl ActionsLoading { get; set; }
        public ObservableCollection<ActionInfo> Actions { get => _actions; set => SetProperty(ref _actions, value); }

        public ICommand PullCmd { get; set; }
        public ICommand SetActionCmd { get; set; }

        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        public HomeViewModel(RaspberryClient client, IPageControl pageControl, IInjectionControl injectionControl)
        {
            _client = client;
            _pageControl = pageControl;
            _injectionControl = injectionControl;

            PullCmd = new Command(Pulled);
            SetActionCmd = new Command(async () => await OpenActionPopupAsync());

            ActionsLoading = new LoadControl
            {
                OnFailAsync = ActionsLoadFailed
            };
            ActionsLoading.Execute(LoadActionsAsync);
        }

        async Task OpenActionPopupAsync()
        {
            var completionSource = new TaskCompletionSource<int>();
            var page = _injectionControl.ResolvePage<DevicePopupViewModel>(vm => vm.TaskCompletionSource = completionSource);
            var currPage = _pageControl.Page as Page;
            await currPage.Navigation.PushModalAsync(page);
            int result = await completionSource.Task;
            await _pageControl.DisplayAlert("Done", result.ToString(), "Ok");
        }

        async Task LoadActionsAsync()
        {
            Actions = new ObservableCollection<ActionInfo>(await _client.GetActionsAsync());
        }

        void Pulled()
        {
            ActionsLoading.Execute(LoadActionsAsync);
            IsRefreshing = false;
        }

        private async Task ActionsLoadFailed(Exception e)
        {
            await _pageControl.DisplayAlert("Error", "Failed fetch actions", "Ok");
            Crashes.TrackError(e);
        }
    }
}
