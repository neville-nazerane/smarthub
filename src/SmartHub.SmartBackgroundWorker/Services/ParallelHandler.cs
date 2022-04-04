using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker.Services
{
    public class ParallelHandler
    {

        private readonly ConcurrentDictionary<Guid, DateTime> _runningTasks = new();
        private readonly ILogger<ParallelHandler> _logger;

        public ParallelHandler(ILogger<ParallelHandler> logger)
        {
            _logger = logger;
        }

        public async Task RunAsync(Func<Task> taskFactory)
        {
            var id = Guid.NewGuid();
            _runningTasks.TryAdd(id, DateTime.UtcNow);
            var task = taskFactory();
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception from task");
            }
            finally
            {
                _runningTasks.Remove(id, out _);
            }
            await RunAsync(taskFactory);
        }

    }
}
