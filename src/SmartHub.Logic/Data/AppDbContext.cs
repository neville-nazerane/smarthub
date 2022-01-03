using Microsoft.EntityFrameworkCore;
using SmartHub.Models;
using SmartHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Logic.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<DeviceAction> DeviceActions { get; set; }

        public DbSet<SceneAction> SceneActions { get; set; }

        public DbSet<EventLog> EventLogs { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

    }
}
