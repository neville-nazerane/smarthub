using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic.Automations
{
    public interface IAutomation
    {

        Task ExecuteAsync(CancellationToken cancellationToken = default);

    }
}
