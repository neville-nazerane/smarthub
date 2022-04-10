using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker.Utils
{
    public class ParallelHandler
    {

        private readonly ConcurrentQueue<Task> _callbacks = new();
        private bool keepRunning;

        public async Task BeginRunningAsync<TResult>(Func<Task<TResult>> runFactory, Func<TResult, Task> callBack)
        {
            keepRunning = true;

            await Task.WhenAll(
                RunTasksAsync(runFactory, callBack),
                RunCallbacksAsync()
                );

        }

        private async Task RunTasksAsync<TResult>(Func<Task<TResult>> runFactory,
                                                          Func<TResult, Task> callBack)
        {
            while (keepRunning)
            {
                var result = await runFactory();
                _callbacks.Enqueue(callBack(result));
            }
        }

        private async Task RunCallbacksAsync()
        {
            while (keepRunning)
            {
                if (_callbacks.TryDequeue(out Task task))
                    await task;
                else
                    await Task.Delay(500);
            }
        }

        public void StopRunning()
        {
            keepRunning = false;
        }


    }
}
