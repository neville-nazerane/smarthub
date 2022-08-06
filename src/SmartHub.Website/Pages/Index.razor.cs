using Microsoft.AspNetCore.Components;
using SmartHub.Constants;
using SmartHub.Consumer;

namespace SmartHub.Website.Pages
{
    public partial class Index
    {


        int frontSpeed = 0;

        int bedroomSpeed = 0;

        [Inject]
        public RaspberryClient RaspberryClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            frontSpeed = await RaspberryClient.GetBondFanSpeedAsync(DeviceConstants.bondFrontFanId);
            bedroomSpeed = await RaspberryClient.GetBondFanSpeedAsync(DeviceConstants.bondBedFanId);

            await base.OnInitializedAsync();
        }

        private async Task OnBedroomSpeedChanged(ChangeEventArgs args) 
            => await UpdateFanSpeedAsync(DeviceConstants.bondBedFanId, int.Parse((string)args.Value));

        private async void OnFrontSpeedChanged(ChangeEventArgs args) 
            => await UpdateFanSpeedAsync(DeviceConstants.bondFrontFanId, int.Parse((string)args.Value));

        private Task UpdateFanSpeedAsync(string fanId, int speed) => RaspberryClient.UpdateBondFanSpeedAsync(fanId, speed);

        private Task PlayTvAsync()
            => RaspberryClient.ExecuteDeviceAsync(DeviceConstants.tvId, new Models.SmartThings.DeviceExecuteModel
            {
                Component = "main",
                Capability = "mediaPlayback",
                Command = "play"
            });

        private Task PauseTvAsync()
            => RaspberryClient.ExecuteDeviceAsync(DeviceConstants.tvId, new Models.SmartThings.DeviceExecuteModel
            {
                Component = "main",
                Capability = "mediaPlayback",
                Command = "pause"
            });

        private Task RewindAsync()
            => RaspberryClient.ExecuteDeviceAsync(DeviceConstants.tvId, new Models.SmartThings.DeviceExecuteModel
            {
                Component = "main",
                Capability = "mediaPlayback",
                Command = "rewind"
            });

        private Task ForwardAsync()
            => RaspberryClient.ExecuteDeviceAsync(DeviceConstants.tvId, new Models.SmartThings.DeviceExecuteModel
            {
                Component = "main",
                Capability = "mediaPlayback",
                Command = "fastForward"
            });

        private Task StopAsync()
            => RaspberryClient.ExecuteDeviceAsync(DeviceConstants.tvId, new Models.SmartThings.DeviceExecuteModel
            {
                Component = "main",
                Capability = "mediaPlayback",
                Command = "stop"
            });


        private Task IncraseVolumeAsync()
            => RaspberryClient.ExecuteDeviceAsync(DeviceConstants.tvId, new Models.SmartThings.DeviceExecuteModel
            {
                Component = "main",
                Capability = "audioVolume",
                Command = "volumeUp"
            });

        private Task DecreaseVolumeAsync()
            => RaspberryClient.ExecuteDeviceAsync(DeviceConstants.tvId, new Models.SmartThings.DeviceExecuteModel
            {
                Component = "main",
                Capability = "audioVolume",
                Command = "volumeDown"
            });

        private Task MuteAsync()
            => RaspberryClient.ExecuteDeviceAsync(DeviceConstants.tvId, new Models.SmartThings.DeviceExecuteModel
            {
                Component = "main",
                Capability = "audioMute",
                Command = "mute"
            });

    }


}
