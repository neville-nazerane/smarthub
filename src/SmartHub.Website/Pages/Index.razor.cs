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
            var frontStatus = await RaspberryClient.GetDeviceStatusAsync(DeviceConstants.frontFanId, "main", "fanSpeed");
            frontSpeed = frontStatus["fanSpeed"]["value"].GetInt32();

            var bedroomStatus = await RaspberryClient.GetDeviceStatusAsync(DeviceConstants.bedFanId, "main", "fanSpeed");
            bedroomSpeed = bedroomStatus["fanSpeed"]["value"].GetInt32();
            
            await base.OnInitializedAsync();
        }

        private async Task OnBedroomSpeedChanged(ChangeEventArgs args)
        {
            await UpdateFanSpeedAsync(DeviceConstants.bedFanId, int.Parse((string)args.Value));
        }

        private async void OnFrontSpeedChanged(ChangeEventArgs args)
        {
            await UpdateFanSpeedAsync(DeviceConstants.frontFanId, int.Parse((string)args.Value));
        }

        private async Task UpdateFanSpeedAsync(string fanId, int speed)
        {
            await RaspberryClient.ExecuteDeviceAsync(fanId, new Models.SmartThings.DeviceExecuteModel
            {
                Capability = "fanSpeed",
                Component = "main",
                Command = "setFanSpeed",
                Arguments = new object[] { speed }
            });

        }

    }
}
