using SmartHub.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.FluentInjector.Utilities;
using Xamarin.Forms;

namespace SmartHub.MobileApp.Utils
{
    public static class PageUtilExtensions
    {

        public static async Task<TResponse> OpenPopupAsync<TViewModel, TResponse>(this IPageControl pageControl, Action<TViewModel> addData = null)
            where TViewModel : PopupViewModelBase<TResponse>
        {
            var completionSource = new TaskCompletionSource<TResponse>();
            await pageControl.PushModalAsync<TViewModel>(vm => {
                vm.TaskCompletionSource = completionSource;
                addData?.Invoke(vm);
            });
            return await completionSource.Task;
        }

    }
}
