using SmartHub.Consumer;
using SmartHub.Models.SmartThings;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.FluentInjector.Utilities;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ViewModels
{
    public class DevicesViewModel : ViewModelBase
    {
        private readonly RaspberryClient _raspberryClient;
        private readonly IInjectionControl _injectionControl;
        private IEnumerable<DeviceItem> _devices;

        public IEnumerable<DeviceItem> Devices { get => _devices; set => SetProperty(ref _devices, value); }

        public ICommand DeviceSelected { get; set; }
        public ICommand OnTest { get; set; }
        public ICommand OffTest { get; set; }

        public DevicesViewModel(RaspberryClient raspberryClient, IInjectionControl injectionControl)
        {
            _raspberryClient = raspberryClient;
            _injectionControl = injectionControl;
            OnTest = new Command<string>(async id => await TestExecute(id, "on"));
            OffTest = new Command<string>(async id => await TestExecute(id, "off"));
            DeviceSelected = new Command<DeviceItem>(async d => await SelectedDeviceAsync(d));
            
            Startup();
        }

        public override async Task InitAsync()
        {
            Devices = await _raspberryClient.GetDevicesAsync();
        }

        private async Task SelectedDeviceAsync(DeviceItem device)
        {
            await _injectionControl.NavigateAsync<DeviceComponentViewModel>(vm =>
            {
                vm.DeviceLabel = device.Label;
                vm.DeviceComponents = device.Components;
                vm.DeviceId = device.DeviceId;
            });
        }

        private async Task TestExecute(string deviceID, string cmd)
        {
            await _raspberryClient.ExecuteDeviceAsync(deviceID, new DeviceExecuteModel
            {
                Capability = "switch",
                Command = cmd,
                Component = "main"
            });
        }

    }
}
