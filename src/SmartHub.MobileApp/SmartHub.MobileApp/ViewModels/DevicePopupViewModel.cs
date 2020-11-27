using SmartHub.MobileApp.Controls;
using SmartHub.MobileApp.Services;
using SmartHub.MobileApp.Utils;
using SmartHub.Models.SmartThings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        private readonly SafeCache<CapabilityData> _capabilityCache;

        protected override int CancelResponse => 0;

        public LoadControl LoadControl { get; set; }

        public IEnumerable<ExpandableData> Devices { get => _devices; set => SetProperty(ref _devices, value); }

        public DevicePopupViewModel(IPageControl pageControl, RaspberryClient raspberryClient) : base(pageControl)
        {
            _raspberryClient = raspberryClient;
            _capabilityCache = new SafeCache<CapabilityData>();
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

        //private async Task<CapabilityData> GetCapabilityAsync(string key)
        //{
        //    CapabilityData cap;
        //    bool isNewTask = false;

        //    var completionSource = _capabilitySafety.GetOrAdd(key, k => {
        //                                isNewTask = true;
        //                                return new TaskCompletionSource<CapabilityData>();
        //                            });
        //    if (isNewTask)
        //    {
        //        try
        //        {
        //            if (!_capabilityCache.TryGetValue(key, out cap))
        //            {
        //                var dataToSend = key.Split("_");
        //                cap = await _raspberryClient.GetCapability(dataToSend[0], float.Parse(dataToSend[1]));
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            completionSource.TrySetException(e);
        //            throw;
        //        }
        //        completionSource.TrySetResult(cap);
        //        _capabilitySafety.Remove(cap);
        //    }
        //    else
        //    {
        //        cap = await completionSource.Task;
        //    }

        //    return cap;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">in format 'id_version'</param>
        /// <returns></returns>
        private Task<CapabilityData> GetCapabilityAsync(string key, CancellationToken cancellationToken = default)
        {
            var dataToSend = key.Split("_");
            return _raspberryClient.GetCapabilityAsync(dataToSend[0], float.Parse(dataToSend[1]), cancellationToken);
        }

        private async Task<IEnumerable<ExpandableData>> GetExpandableCommandsAsync(string key)
        {
            var capability = await _capabilityCache.GetDataAsync(key, GetCapabilityAsync);
            return capability.GetCommands().Select(c => new ExpandableData
            {
                Text = c
            });
        }

        private Task<IEnumerable<ExpandableData>> GetExpandablesAsync(DeviceItem deviceItem)
        {
            var result = deviceItem.Components
                                   .Select(comp => comp.Capabilities
                                                       .Select(cap => new { 
                                                            Text = $"{comp.Id} - {cap.Id}",
                                                            Key = $"{cap.Id}_{cap.Version}"
                                                       }))
                                   .SelectMany(s => s)
                                   .Select(s => new ExpandableData
                                   {
                                       Text = s.Text,
                                       LoadControl = new CollectionLoadControl { 
                                            OnExecuteAsync = async () => await GetExpandableCommandsAsync(s.Key)
                                       }
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
