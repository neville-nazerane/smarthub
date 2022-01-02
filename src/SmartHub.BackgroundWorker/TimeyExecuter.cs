using Microsoft.Extensions.Options;
using SmartHub.Logic;
using SmartHub.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.BackgroundWorker
{
    public class TimeyExecuter
    {
        private readonly IOptions<GlobalConfig> _globalOptions;
        private readonly AzBackupClient _azBackupClient;

        public TimeyExecuter( IOptions<GlobalConfig> globalOptions, AzBackupClient azBackupClient)
        {
            _globalOptions = globalOptions;
            _azBackupClient = azBackupClient;
        }

        public async Task RunAsync(TimeSpan logInterval, CancellationToken cancellationToken = default)
        {
            //GZipStream.
            //ZipFile.CreateFromDirectory(_globalOptions.Value.DataPath);

            if (!Directory.Exists("temps"))
                Directory.CreateDirectory("temps");

            string fileName = $"temps/{Guid.NewGuid()}.zip";
            ZipFile.CreateFromDirectory(_globalOptions.Value.DataPath, fileName);

            try
            {
                await using var stream = File.OpenRead(fileName);
                await _azBackupClient.UploadAsync(stream, cancellationToken);
            }
            finally
            {
                File.Delete(fileName);
            }
            //await _eventLogService.ClearLogsAsync(logInterval, cancellationToken);
        }

    }
}
