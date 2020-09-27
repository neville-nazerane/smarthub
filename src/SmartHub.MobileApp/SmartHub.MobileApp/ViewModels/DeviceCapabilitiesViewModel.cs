using SmartHub.MobileApp.Pages;
using SmartHub.MobileApp.Services;
using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.FluentInjector.Utilities;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ViewModels
{
    public class DeviceCapabilitiesViewModel
    {
        private readonly IInjectionControl _injectionControl;
        private readonly RaspberryClient _raspberryClient;

        public string DeviceLabel { get; set; }
        public string DeviceId { get; set; }

        public string ComponentLabel { get; set; }

        public IEnumerable<DeviceComponent> PastDeviceComponents { get; set; }


        public IEnumerable<DeviceCapability> DeviceCapabilities { get; set; }

        public ICommand ToDevices { get; set; }
        public ICommand ToDeviceComponent { get; set; }

        public ICommand OnTest { get; set; }
        public ICommand OffTest { get; set; }

        public DeviceCapabilitiesViewModel(IInjectionControl injectionControl, RaspberryClient raspberryClient)
        {
            ToDevices = new Command(async () => await injectionControl.NavigateAsync<DevicesPage>());
            ToDeviceComponent = new Command(async () => await ToDeviceComponentAsync());
            OnTest = new Command<DeviceCapability>(async c => await TestExecute(c, "on"));
            OffTest = new Command<DeviceCapability>(async c => await TestExecute(c, "off"));
            _injectionControl = injectionControl;
            _raspberryClient = raspberryClient;
        }

        private async Task ToDeviceComponentAsync()
        {
            await _injectionControl.NavigateAsync<DeviceComponentViewModel>(vm =>
            {
                vm.DeviceLabel = DeviceLabel;
                vm.DeviceComponents = PastDeviceComponents;
                vm.DeviceId = DeviceId;
            });
        }

        private async Task TestExecute(DeviceCapability deviceCapability, string cmd)
        {
            await _raspberryClient.ExecuteDeviceAsync(DeviceId, new DeviceExecuteModel { 
                Capability = deviceCapability.Id,
                Command = cmd,
                Component = ComponentLabel
            });
        }

    }
}
