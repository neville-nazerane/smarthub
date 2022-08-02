using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker.Utils
{
    internal static class SystemExtensions
    {

        public static DateTime ClearSeconds(this DateTime dateTime)
            => new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);


    }
}
