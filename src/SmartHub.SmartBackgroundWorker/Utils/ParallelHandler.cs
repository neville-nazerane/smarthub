using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker.Utils
{

    public abstract class CallbackHandler
    {
        protected bool keepRunning;

        public async static Task<CallbackHandler> BeginRunningAsync<TResult>
                                    (Func<CancellationToken, Task<TResult>> runFactory, Func<TResult, CancellationToken, Task> callBack)
        {
            var handler = new CallbackHandlerForTask();
            await handler.RunAsync(runFactory, callBack);
            return handler;
        }

        public async static Task<CallbackHandler> BeginRunningAsync<TResult>
                            (Func<CancellationToken, Task<TResult>> runFactory, Func<TResult, CancellationToken, ValueTask> callBack)
        {
            var handler = new CallbackHandlerForValueTask();
            await handler.RunAsync(runFactory, callBack);
            return handler;
        }

        class CallbackHandlerForValueTask : CallbackHandler<ValueTask>
        {
            protected override async Task AwaitCallback(ValueTask task) => await task;
        }

        class CallbackHandlerForTask : CallbackHandler<Task>
        {
            protected override Task AwaitCallback(Task task) => task;

        }

    }

    public abstract class CallbackHandler<TCallback> : CallbackHandler
    {

        private readonly ConcurrentQueue<TCallback> _callbacks = new();

        protected CallbackHandler()
        {

        }

        public async Task RunAsync<TResult>(Func<CancellationToken, Task<TResult>> runFactory,
                                            Func<TResult, CancellationToken, TCallback> callBack,
                                            CancellationToken cancellationToken = default)
        {
            keepRunning = true;

            await Task.WhenAll(
                RunTasksAsync(runFactory, callBack, cancellationToken),
                RunCallbacksAsync()
                );
        }

        private async Task RunTasksAsync<TResult>(Func<CancellationToken, Task<TResult>> runFactory,
                                                  Func<TResult, CancellationToken, TCallback> callBack,
                                                  CancellationToken cancellationToken = default)
        {
            while (keepRunning)
            {
                var result = await runFactory(cancellationToken);
                _callbacks.Enqueue(callBack(result, cancellationToken));
            }
        }

        private async Task RunCallbacksAsync()
        {
            while (keepRunning)
            {
                if (_callbacks.TryDequeue(out TCallback task))
                    await AwaitCallback(task);
                else
                    await Task.Delay(500);
            }
        }

        protected abstract Task AwaitCallback(TCallback task);

    }
}
