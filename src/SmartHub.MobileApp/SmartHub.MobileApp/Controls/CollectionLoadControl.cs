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

        private IEnumerable<object> itemsCache;

        private IEnumerable<object> _items;

        public IEnumerable<object> Items { get => _items; private set => SetProperty(ref _items, value); }

        public Func<Task<IEnumerable<object>>> OnExecuteAsync;
        private bool _isExpanded;

        public ICommand ExpandOrCollapseCmd { get; set; }

        public bool IsExpanded { get => _isExpanded; private set => SetProperty(ref _isExpanded, value); }

        public bool UseCache { get; set; }

        public CollectionLoadControl()
        {
            ExpandOrCollapseCmd = new Command(async () => await ExpandOrCollapseAsync());
        }

        private async Task ExecuteAndCacheAsync()
        {
            if (OnExecuteAsync is not null)
            {
                itemsCache = await ExecuteAsync(OnExecuteAsync);
            }
        }

        public async Task ExpandOrCollapseAsync()
        {
            if (IsExpanded)
                Collapse();
            else await ExpandAsync();
        }

        public async Task ExpandAsync()
        {
            if (itemsCache is null)
                await ExecuteAndCacheAsync();
            Items = itemsCache;
            IsExpanded = true;
        }

        public void Collapse()
        {
            Items = null;
            if (!UseCache) itemsCache = null;
            IsExpanded = false;
        }
    }
}
