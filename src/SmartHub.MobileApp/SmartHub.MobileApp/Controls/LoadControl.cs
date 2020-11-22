using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.MobileApp.Controls
{
    public class LoadControl : BindableModel
    {
        private bool _isLoading;
        
        public bool IsLoading
        {
            get => _isLoading; 
            set
            {
                SetProperty(ref _isLoading, value);
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }
        public bool IsNotLoading => !IsLoading;

        public Func<Exception, Task> OnFailAsync { private get; set; }

        public LoadControl()
        {
            IsLoading = false;
        }

        public async Task ExecuteAsync(Func<Task> compute)
        {
            using var _ = StartLoading();
            try
            {
                await compute();
            }
            catch (Exception e)
            {
                await OnFailAsync(e);
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> compute)
        {
            using var _ = StartLoading();
            try
            {
                return await compute();
            }
            catch (Exception e)
            {
                await OnFailAsync(e);
                throw;
            }
        }

        public void Execute(Func<Task> compute)
        {
            IsLoading = true;
            Task.Run(() => {
                TryRunAsync(compute).ContinueWith(o => {
                    IsLoading = false;
                    return Task.CompletedTask;
                });
            });
        }

        private async Task TryRunAsync(Func<Task> compute)
        {
            try
            {
                await compute();
            }
            catch (Exception e)
            {
                if (OnFailAsync is not null)
                    await OnFailAsync(e);
            }
        }
        IDisposable StartLoading() => new Loader(this);
        class Loader : IDisposable
        {
            private readonly LoadControl _src;

            public Loader(LoadControl src)
            {
                _src = src;
                _src.IsLoading = true;
            }

            public void Dispose()
            {
                _src.IsLoading = false;
            }
        }

    }
}
