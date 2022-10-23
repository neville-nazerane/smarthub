using Microsoft.AspNetCore.Components;
using SmartHub.Constants;
using SmartHub.Consumer;

namespace SmartHub.Website.Pages
{
    public partial class Hue
    {

        [Inject]
        public RaspberryClient Client { get; set; }

        bool? leftBarIsOn = null;
        bool? rightBarIsOn = null;
        bool? leftHaloIsOn = null;
        bool? rightHaloIsOn = null;
        bool? plugIsOn = null;
        bool? mainBedroomOn = null;

        protected override async Task OnInitializedAsync()
        {
            leftBarIsOn = await Client.GetHueLight(DeviceConstants.computerLeftBarId);
            rightBarIsOn = await Client.GetHueLight(DeviceConstants.computerRightBarId);
            leftHaloIsOn = await Client.GetHueLight(DeviceConstants.computerLeftHaloId);
            rightHaloIsOn = await Client.GetHueLight(DeviceConstants.computerRightHaloId);
            plugIsOn = await Client.GetHueLight(DeviceConstants.hueComputerLightPlugId);
            mainBedroomOn = await Client.GetBondLightAsync(DeviceConstants.bondBedFanId);
        }

        string ClassCheck(bool? status) => status is null ? "secondary" : (status.Value ? "primary" : "danger");

        Task RunTogetherAsync(params Func<Task>[] tasks)
            => Task.WhenAll(tasks.Select(t => t()));

        Task SwitchLeftBarAsync()
            => SwitchLightAsync(DeviceConstants.computerLeftBarId, ref leftBarIsOn);

        Task SwitchRightBarAsync()
            => SwitchLightAsync(DeviceConstants.computerRightBarId, ref rightBarIsOn);

        Task SwitchLeftHaloAsync()
            => SwitchLightAsync(DeviceConstants.computerLeftHaloId, ref leftHaloIsOn);

        Task SwitchRightHaloAsync()
            => SwitchLightAsync(DeviceConstants.computerRightHaloId, ref rightHaloIsOn);

        Task SwitchPlugAsync()
            => SwitchLightAsync(DeviceConstants.hueComputerLightPlugId, ref plugIsOn);

        Task SwitchMainAsync()
            => SwitchBondLightAsync(DeviceConstants.bondBedFanId, ref mainBedroomOn);

        Task SwitchBondLightAsync(string id, ref bool? switcher)
            => Client.SwitchBondLightAsync(id, (switcher = !switcher.Value).Value);

        Task SwitchLightAsync(string id, ref bool? switcher)
            => Client.SetHueLight(id, (switcher = !switcher.Value).Value);

    }
}
