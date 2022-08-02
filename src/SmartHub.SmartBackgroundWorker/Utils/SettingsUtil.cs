using Microsoft.EntityFrameworkCore;
using SmartHub.Logic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker.Utils
{
    public static class SettingsUtil
    {

        public static async Task<int> GetSettingAsIntAsync(this AppDbContext dbContext, string key, CancellationToken cancellationToken = default)
        {
            string res = await dbContext.GetSettingAsync(key, cancellationToken);
            if (res is null)
                throw new KeyNotFoundException(key);
            return int.Parse(res);
        }

        public static Task<string> GetSettingAsync(this AppDbContext dbContext, string key, CancellationToken cancellationToken = default)
            => dbContext.Settings.Where(s => s.Name == key)
                                 .Select(s => s.Value)
                                 .SingleOrDefaultAsync(cancellationToken);



    }
}
