using SmartHub.Consumer;
using SmartHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartHub.MobileApp.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly RaspberryClient _raspberryClient;
        private ObservableCollection<Setting> settings;
        private bool isRefreshing;

        public ObservableCollection<Setting> Settings { get => settings; set => SetProperty(ref settings, value); }

        public Command PullCommand { get; set; }

        public Command UpdateCommand { get; set; }

        public bool IsRefreshing { get => isRefreshing; set => SetProperty(ref isRefreshing, value); }

        public SettingsViewModel(RaspberryClient raspberryClient)
        {
            _raspberryClient = raspberryClient;
            PullCommand = new Command(async () => await PullAsync());
            UpdateCommand = new Command<Setting>(async s => await UpdateAsync(s));

            Startup();
        }

        private async Task PullAsync()
        {
            IsRefreshing = true;
            try
            {
                await UpdateSettingsAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task UpdateAsync(Setting setting)
        {
            IsLoading = true;
            try
            {
                await _raspberryClient.UpdateSettingAsync(setting);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public override Task InitAsync() => UpdateSettingsAsync();

        public async Task UpdateSettingsAsync()
        {
            var res = await _raspberryClient.GetSettingsAsync();
            Settings = new ObservableCollection<Setting>(res);
        }

    }
}
