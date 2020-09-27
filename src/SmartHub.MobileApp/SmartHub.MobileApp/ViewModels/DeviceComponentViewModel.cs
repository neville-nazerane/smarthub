using SmartHub.MobileApp.Pages;
using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.FluentInjector.Utilities;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ViewModels
{
    public class DeviceComponentViewModel : ViewModelBase
    {
        private readonly IInjectionControl _injectionControl;

        public string DeviceLabel { get; set; }

        public IEnumerable<DeviceComponent> DeviceComponents { get; set; }

        public ICommand ToDevices { get; set; }

        public ICommand SelectedComponent { get; set; }
        public string DeviceId { get; internal set; }

        public DeviceComponentViewModel(IInjectionControl injectionControl)
        {
            ToDevices = new Command(async () => await injectionControl.NavigateAsync<DevicesPage>());
            SelectedComponent = new Command<DeviceComponent>(async c => await SelectedComponentAsync(c));
            _injectionControl = injectionControl;
        }

        private async Task SelectedComponentAsync(DeviceComponent component)
        {
            await _injectionControl.NavigateAsync<DeviceCapabilitiesViewModel>(vm => {
                vm.DeviceLabel = DeviceLabel;
                vm.ComponentLabel = component.Id;
                vm.PastDeviceComponents = DeviceComponents;
                vm.DeviceCapabilities = component.Capabilities;
                vm.DeviceId = DeviceId;
            });
        }

    }
}
