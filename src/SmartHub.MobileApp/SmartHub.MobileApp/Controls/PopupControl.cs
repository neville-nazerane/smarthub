using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.MobileApp.Controls
{
    public class PopupControl<TResult>
    {

        private readonly TaskCompletionSource<TResult> _completionSource;

        public PopupControl()
        {
            _completionSource = new TaskCompletionSource<TResult>();
        }

        public bool SetResult(TResult result) => _completionSource.TrySetResult(result);

        public bool Throw(Exception e) => _completionSource.TrySetException(e);

    }
}
