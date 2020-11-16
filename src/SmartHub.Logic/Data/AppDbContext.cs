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
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //public DbSet<MappedDevice> MappedDevices { get; set; }

        public DbSet<DeviceAction> DeviceActions { get; set; }

    }
}
