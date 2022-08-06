using Microsoft.AspNetCore.Components;
using SmartHub.Consumer;
using SmartHub.Models.Entities;

namespace SmartHub.Website.Pages
{
    public partial class Scenes
    {
        private IEnumerable<SceneState> scenes;

        [Inject]
        public RaspberryClient Client { get; set; }

        protected override async Task OnInitializedAsync()
        {
            scenes = await Client.GetScenesAsync();
            await base.OnInitializedAsync();
        }

        async Task SwapAsync(SceneState scene)
        {
            await Client.UpdateSceneAsync(scene.GetNameAsEnum(), scene.IsEnabled);
            scenes = await Client.GetScenesAsync();
        }

    }
}
