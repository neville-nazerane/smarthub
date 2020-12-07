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

        static readonly ConcurrentDictionary<string, TaskCompletionSource> _eventLocker;

        static EventService()
        {
            _eventLocker = new ConcurrentDictionary<string, TaskCompletionSource>();
        }

        public EventService(AutomationService automationService, EventLogService eventLogService)
        {
            _automationService = automationService;
            _eventLogService = eventLogService;
        }

        public async Task RecieveAsync(string @event, CancellationToken cancellationToken = default)
        {
            await WaitForEventAsync(@event);
            try
            {
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
            bool isCreated = false;
            var completionSource = _eventLocker.GetOrAdd(eventName, key => {
                                        isCreated = true;
                                        return new TaskCompletionSource();
                                    });
            if (!isCreated)
                return completionSource.Task;
            else return Task.CompletedTask;
        }

        private static void ReleaseEvent(string eventName)
        {
            _eventLocker.TryGetValue(eventName, out var taskCompletionSource);
            taskCompletionSource.TrySetResult();
        }

    }
}
