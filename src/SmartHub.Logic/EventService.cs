using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class EventService
    {
        private readonly AutomationService _automationService;
        private readonly EventLogService _eventLogService;
        private readonly SmartLogic _smartLogic;
        static readonly ConcurrentDictionary<string, ConcurrentQueue<TaskCompletionSource>> _lockers;

        //static readonly ConcurrentDictionary<string, TaskCompletionSource> _eventLocker;

        static EventService()
        {
            //_eventLocker = new ConcurrentDictionary<string, TaskCompletionSource>();
            _lockers = new ConcurrentDictionary<string, ConcurrentQueue<TaskCompletionSource>>();
        }

        public EventService(AutomationService automationService, EventLogService eventLogService, SmartLogic smartLogic)
        {
            _automationService = automationService;
            _eventLogService = eventLogService;
            _smartLogic = smartLogic;
        }

        public async Task RecieveAsync(string @event, CancellationToken cancellationToken = default)
        {
            await WaitForEventAsync(@event);
            try
            {
                await _smartLogic.ExecuteRawAutomation(@event, cancellationToken);
                await _eventLogService.LogAsync(@event, cancellationToken);
                await _automationService.ExecuteForAsync(@event, cancellationToken);
            }
            catch
            {
                throw;
            }
            finally
            {
                ReleaseEvent(@event);
            }
        }

        private static Task WaitForEventAsync(string eventName)
        {
            var sources = _lockers.GetOrAdd(eventName, k => new ConcurrentQueue<TaskCompletionSource>());
            var tasks = sources.Select(s => s.Task).ToArray();
            sources.Enqueue(new TaskCompletionSource());

            return Task.WhenAll(tasks);

            //bool isCreated = false;
            //var completionSource = _eventLocker.GetOrAdd(eventName, key => {
            //                            isCreated = true;
            //                            return new TaskCompletionSource();
            //                        });
            //if (!isCreated)
            //    return completionSource.Task;
            //else return Task.CompletedTask;
        }

        private static void ReleaseEvent(string eventName)
        {
            if(_lockers.TryGetValue(eventName, out var sources) 
                && sources.TryDequeue(out var completionSource))
                completionSource.TrySetResult();

            //_eventLocker.TryGetValue(eventName, out var taskCompletionSource);
            //taskCompletionSource.TrySetResult();
        }

    }
}
