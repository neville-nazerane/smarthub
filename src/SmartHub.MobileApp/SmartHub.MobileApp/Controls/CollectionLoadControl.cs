using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartHub.MobileApp.Controls
{
    public class CollectionLoadControl : LoadControl
    {
        private IEnumerable<object> _items;

        public IEnumerable<object> Items { get => _items; private set => SetProperty(ref _items, value); }

        public Func<Task<IEnumerable<object>>> OnExecuteAsync;

        public ICommand ExecuteCmd { get; set; }

        public CollectionLoadControl()
        {
            ExecuteCmd = new Command(async () => await ExecuteAsync());
        }

        public async Task ExecuteAsync()
        {
            if (OnExecuteAsync is not null)
            {
                Items = await ExecuteAsync(OnExecuteAsync);
            }
        }

    }
}
