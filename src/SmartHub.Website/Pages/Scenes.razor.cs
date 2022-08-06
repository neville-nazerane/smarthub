using SmartHub.Consumer;
using SmartHub.Models.Entities;

namespace SmartHub.Website.Pages
{
    public partial class Scenes
    {
        private IEnumerable<SceneState> scenes;

        public RaspberryClient Client { get; set; }

        protected override async Task OnInitializedAsync()
        {
            scenes = await Client.GetScenesAsync();
            await base.OnInitializedAsync();
        }

        Task SwapAsync(SceneState scene)
        {
            scene.IsEnabled = !scene.IsEnabled;
            return Client.UpdateSceneAsync(scene.GetNameAsEnum(), scene.IsEnabled);
        }

    }
}
