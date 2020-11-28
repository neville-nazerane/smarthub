using SmartHub.MobileApp.Controls;
using SmartHub.MobileApp.Models;
using SmartHub.MobileApp.Services;
using SmartHub.MobileApp.Utils;
using SmartHub.Models.Entities;
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
    public class DevicePopupViewModel : PopupViewModelBase<IActionSaveModel>
    {
        private readonly IPageControl _pageControl;
        private readonly RaspberryClient _raspberryClient;
        private IEnumerable<ExpandableData> _devices;
        private bool _isDevicesDisplayed;
        private IEnumerable<SceneItem> _scenes;
        private readonly SafeCache<CapabilityData> _capabilityCache;

        protected override IActionSaveModel CancelResponse => null;

        public bool IsDevicesDisplayed { get => _isDevicesDisplayed; set => SetProperty(ref _isDevicesDisplayed, value); }

        public LoadControl LoadControl { get; set; }
        public ICommand DeleteCmd { get; set; }
        public ICommand SubmitScene { get; set; }
        public ICommand SubmitDevice { get; set; }
        public ICommand SwitchTabCmd { get; set; }

        public IEnumerable<SceneItem> Scenes { get => _scenes; set =>  SetProperty(ref _scenes, value); }
        public IEnumerable<ExpandableData> Devices { get => _devices; set => SetProperty(ref _devices, value); }

        public DevicePopupViewModel(IPageControl pageControl, RaspberryClient raspberryClient) : base(pageControl)
        {
            _pageControl = pageControl;
            _raspberryClient = raspberryClient;
            _capabilityCache = new SafeCache<CapabilityData>();
            LoadControl = new LoadControl();
            LoadControl.Execute(SetupDevices);
            IsDevicesDisplayed = true;

            DeleteCmd = new Command(async () => await DeleteAsync());
            SubmitDevice = new Command<DeviceAction>(async a => await SetResultAsync(new DeviceActionSaveModel(a)));
            SubmitScene = new Command<string>(async id => await SetResultAsync(new SceneActionSaveModel(id)));
            SwitchTabCmd = new Command<string>(k => IsDevicesDisplayed = k == "devices");
        }

        private async Task DeleteAsync()
        {
            if (await _pageControl.DisplayAlert("Remove?", "Are you sure you want to remove linking?", "Remove", "No"))
            {
                await SetResultAsync(ActionDeleteModel.Instance);
            }
        }

        private async Task SetupDevices()
        {
            Scenes = await _raspberryClient.GetScenesAsync();
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

        private async Task<IEnumerable<DeviceAction>> GetExpandableCommandsAsync(string key, DeviceAction action)
        {
            var capability = await _capabilityCache.GetDataAsync(key, GetCapabilityAsync);
            return capability.GetCommands().Select(c => new DeviceAction
            {
                DeviceId = action.DeviceId,
                Component = action.Component,
                Capability = action.Capability,
                Command = c
            });
        }

        private Task<IEnumerable<ExpandableData>> GetExpandablesAsync(DeviceItem deviceItem)
        {
            var result = deviceItem.Components
                                   .Select(comp => comp.Capabilities
                                                       .Select(cap => new
                                                       {
                                                           Text = $"{comp.Id} - {cap.Id}",
                                                           Key = $"{cap.Id}_{cap.Version}",
                                                           Action = new DeviceAction
                                                           {
                                                               DeviceId = deviceItem.DeviceId,
                                                               Component = comp.Id,
                                                               Capability = cap.Id
                                                           }
                                                       }))
                                   .SelectMany(s => s)
                                   .Select(s => new ExpandableData
                                   {
                                       Text = s.Text,
                                       LoadControl = new CollectionLoadControl
                                       {
                                           OnExecuteAsync = async () => await GetExpandableCommandsAsync(s.Key, s.Action)
                                       }
                                   });

            return Task.FromResult(result);
        }

    }
}
