using SmartHub.MobileApp.Services;
using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ViewModels
{
    public class DevicesViewModel : ViewModelBase
    {
        private readonly RaspberryClient _raspberryClient;

        public IEnumerable<DeviceItem> Devices { get; set; }

        public DevicesViewModel(RaspberryClient raspberryClient)
        {
            _raspberryClient = raspberryClient;
            //Startup();
        }

        public override async Task InitAsync()
        {
            Devices = await _raspberryClient.GetDevicesAsync();
        }

    }
}
