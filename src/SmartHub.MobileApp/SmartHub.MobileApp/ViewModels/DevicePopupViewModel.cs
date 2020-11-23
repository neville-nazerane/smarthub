using SmartHub.MobileApp.Controls;
using SmartHub.MobileApp.Services;
using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.FluentInjector.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace SmartHub.MobileApp.ViewModels
{
    public class DevicePopupViewModel : PopupViewModelBase<int>
    {
        private readonly RaspberryClient _raspberryClient;
        private IEnumerable<ExpandableData> _devices;

        protected override int CancelResponse => 0;

        public LoadControl LoadControl { get; set; }

        public IEnumerable<ExpandableData> Devices { get => _devices; set => SetProperty(ref _devices, value); }

        public DevicePopupViewModel(IPageControl pageControl, RaspberryClient raspberryClient) : base(pageControl)
        {
            _raspberryClient = raspberryClient;
            LoadControl = new LoadControl();
            LoadControl.Execute(SetupDevices);
        }

        private async Task SetupDevices()
        {
            var devices = await _raspberryClient.GetDevicesAsync();
            Devices = devices.Select(d => new ExpandableData
            {
                Text = d.Label,
                LoadControl = new CollectionLoadControl
                {
                    OnExecuteAsync = async () => await GetExpandablesAsync(d)
                }
            });
        }

        private Task<IEnumerable<ExpandableData>> GetExpandablesAsync(DeviceItem deviceItem)
        {
            var result = deviceItem.Components
                                   .Select(comp => comp.Capabilities
                                                       .Select(cap => $"{comp.Id} - {cap.Id}"))
                                   .SelectMany(s => s)
                                   .Select(s => new ExpandableData
                                   {
                                       Text = s
                                   });

            return Task.FromResult(result);
        }

        class ExecuteContext
        {
            public string DeviceId { get; set; }

            public DeviceExecuteModel ExecuteModel { get; set; }

        }

    }
}
