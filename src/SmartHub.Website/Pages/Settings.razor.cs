using Microsoft.AspNetCore.Components;
using SmartHub.Models.Entities;
using System.Net.Http.Json;

namespace SmartHub.Website.Pages
{
    public partial class Settings
    {

        IEnumerable<Setting> settings;

        [Inject]
        public HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            settings = await HttpClient.GetFromJsonAsync<IEnumerable<Setting>>("settings");
        }

        Task SaveAsync(Setting setting) => HttpClient.PutAsJsonAsync("settings", setting);

    }
}
