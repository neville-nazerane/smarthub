using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.FluentInjector.Utilities;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ViewModels
{
    public abstract class PopupViewModelBase<TResponse> : ViewModelBase
    {

        public TaskCompletionSource<TResponse> TaskCompletionSource { private get; set; }
        protected IPageControl PageControl { get; }

        public ICommand CloseCmd { get; set; }

        public ICommand SubmitCmd { get; set; }

        protected abstract TResponse CancelResponse { get; }

        public PopupViewModelBase(IPageControl pageControl)
        {
            CloseCmd = new Command(async () => await SetResultAsync(CancelResponse));
            SubmitCmd = new Command<TResponse>(async response => await SetResultAsync(response));
            PageControl = pageControl;
        }

        public void Cancel() => TaskCompletionSource.TrySetResult(CancelResponse);

        protected async Task<bool> SetResultAsync(TResponse response)
        {
            await PageControl.PopModalAsync();
            return TaskCompletionSource.TrySetResult(response);
        }

        protected async Task<bool> ThrowExceptionAsync(Exception e)
        {
            await PageControl.PopModalAsync();
            return TaskCompletionSource.TrySetException(e);
        }
    }
}
