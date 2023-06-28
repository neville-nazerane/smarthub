using SmartHub.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker.Services
{
    public class HueSyncProcess
    {
        private readonly HueSyncClient _client;

        public HueSyncProcess(HueSyncClient client)
        {
            _client = client;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            //await _client.UpdateExecuteAsync(new()
            //{
            //    SyncActive = true
            //}, cancellationToken);
            var result = await _client.GetExecutionAsync(cancellationToken);
            if (result.HdmiActive && result.HdmiSource == "input3" && !result.SyncActive)
            {
                //await _client.UpdateExecuteAsync(new()
                //{
                //    SyncActive = true
                //}, cancellationToken);
            }
        }

    }
}
