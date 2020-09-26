using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.MobileApp.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isLoading;

        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        protected void Startup()
        {
            Task.Run(() =>
            {
                IsLoading = true;
                InitAsync().ContinueWith(t => IsLoading = false);
            });
        }

        public virtual Task InitAsync() => Task.CompletedTask;

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
