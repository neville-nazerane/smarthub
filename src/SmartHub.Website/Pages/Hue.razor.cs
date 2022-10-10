﻿using Microsoft.AspNetCore.Components;
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

        protected override async Task OnInitializedAsync()
        {

            await RunTogetherAsync(
                 async () => leftBarIsOn = await Client.GetHueLight(DeviceConstants.computerLeftBarId),
                 async () => rightBarIsOn = await Client.GetHueLight(DeviceConstants.computerRightBarId),
                 async () => leftHaloIsOn = await Client.GetHueLight(DeviceConstants.computerLeftHaloId),
                 async () => rightHaloIsOn = await Client.GetHueLight(DeviceConstants.computerRightHaloId),
                 async () => plugIsOn = await Client.GetHueLight(DeviceConstants.hueComputerLightPlugId)
            );
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

        Task SwitchLightAsync(string id, ref bool? switcher)
            => Client.SetHueLight(id, (switcher = !switcher.Value).Value);

    }
}
