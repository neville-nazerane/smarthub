using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker.Utils
{
    public class MinuiteProcessor
    {

        private readonly ConcurrentDictionary<DateTime, ICollection<Context>> _toExecute;
        private readonly ILogger<MinuiteProcessor> _logger;

        private TaskCompletionSource locker;

        public MinuiteProcessor(ILogger<MinuiteProcessor> logger)
        {
            _toExecute = new();
            _logger = logger;
        }

        public async Task AddAsync(string label, DateTime executionTime, Func<Task> taskFactory)
        {
            var model = new Context
            {
                TimeStamp = executionTime,
                Label = label,
                TaskFactory = taskFactory
            };

            using (await BeginLockAsync())
            {
                InternalRemove(label);
                var list = _toExecute.GetOrAdd(executionTime, t => new List<Context>());
                list.Add(model);
            }
        }

        public async Task RemoveAsync(string label)
        {
            using (await BeginLockAsync())
            {
                InternalRemove(label);
            }
        }
        
        private void InternalRemove(string label)
        {
            var existingItem = _toExecute.Values.SelectMany(v => v)
                                                 .SingleOrDefault(v => v.Label == label);
            if (existingItem is not null)
                _toExecute[existingItem.TimeStamp].Remove(existingItem);
            //_toExecute.TryRemove(existingItem.TimeStamp, out _);
        }

        public async Task RunByDateAsync(DateTime execution)
        {
            IEnumerable<Context> contexts = Array.Empty<Context>();
            using (await BeginLockAsync())
            {
                var times = _toExecute.Keys.Where(t => t <= execution).ToList();
                foreach (var time in times)
                {
                    _toExecute.TryGetValue(time, out var ctx);
                    _toExecute.TryRemove(time, out _);
                    contexts = contexts.Union(ctx);
                }
            }
            foreach (var context in contexts)
            {
                _logger.LogInformation("Running {label}", context.Label);
                try
                {
                    await context.TaskFactory();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to run {label}", context.Label);
                }
            }
        }

        public Task RunForNowAsync() => RunByDateAsync(DateTime.Now);

        private async Task<IDisposable> BeginLockAsync()
        {
            if (locker is not null) await locker.Task;
            return new LockerUp(this);
        }

        private void ReleaseLock()
        {
            if (locker is null) return;
            locker.SetResult();
            locker = null;
        }

        class LockerUp : IDisposable
        {

            private readonly MinuiteProcessor _processor;

            public LockerUp(MinuiteProcessor processor)
            {
                _processor = processor;
            }

            public void Dispose()
            {
                _processor.ReleaseLock();
            }
        }


        class Context
        {
            public Func<Task> TaskFactory { get; set; }

            public DateTime TimeStamp { get; set; }

            public string Label { get; set; }
        }

    }
}
