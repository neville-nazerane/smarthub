using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;

namespace SmartHub.MobileApp.Utils
{
    public class SafeCache<TData>
        where TData : class
    {

        private readonly Dictionary<string, TData> _cache;

        private readonly ConcurrentDictionary<string, TaskCompletionSource<TData>> _taskcheck;

        public SafeCache()
        {
            _cache = new Dictionary<string, TData>();
            _taskcheck = new ConcurrentDictionary<string, TaskCompletionSource<TData>>();
        }

        public Task<TData> GetDataAsync(string key, Func<string, Task<TData>> dataFactory) => GetDataAsync(key, async (key, cancel) => await dataFactory(key));

        public async Task<TData> GetDataAsync(string key, Func<string, CancellationToken, Task<TData>> dataFactory, CancellationToken cancellationToken = default)
        {
            TData result;
            bool isNewTask = false;
            var completionSource = _taskcheck.GetOrAdd(key, k =>
                                                {
                                                    isNewTask = true;
                                                    return new TaskCompletionSource<TData>();
                                                });

            if (isNewTask)
            {
                try
                {
                    if (!_cache.TryGetValue(key, out result))
                    {
                        result = await dataFactory(key, cancellationToken);
                        _cache.Add(key, result);
                    }
                    completionSource.TrySetResult(result);
                    _taskcheck.TryRemove(key, out _);
                }
                catch (Exception e)
                {
                    completionSource.TrySetException(e);
                    throw;
                }
            }
            else
                result = await completionSource.Task;
            return result;
        }

    }
}
