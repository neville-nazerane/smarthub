using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.FluentInjector.Utilities;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ViewModels
{
    public class DevicePopupViewModel : ViewModelBase
    {
        private readonly IPageControl _pageControl;

        public TaskCompletionSource<int> TaskCompletionSource { get; set; }

        public ICommand CloseCmd { get; set; }

        public DevicePopupViewModel(IPageControl pageControl)
        {
            CloseCmd = new Command(async () => await CloseAsync());
            _pageControl = pageControl;
        }

        async Task CloseAsync()
        {
            TaskCompletionSource.TrySetResult(23);
            var currPage = _pageControl.Page as Page;
            await currPage.Navigation.PopModalAsync();
        }

    }
}
