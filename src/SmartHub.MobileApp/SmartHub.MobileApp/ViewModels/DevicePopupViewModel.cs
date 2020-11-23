using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.FluentInjector.Utilities;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ViewModels
{
    public class DevicePopupViewModel : PopupViewModelBase<int>
    {

        protected override int CancelResponse => 0;

        public DevicePopupViewModel(IPageControl pageControl) : base(pageControl)
        {
        }

    }
}
