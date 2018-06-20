using System;
using System.Collections.Generic;
using System.Text;
using JFJT.Framework.EntityFramework;
using LiveFactory.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveFactory.EntityFramework
{
    public class LiveFactoryDbContext : AppDbContext
    {
        public DbSet<LiveChannel> LiveChannels { get; set; }
        public DbSet<PlayInfo> PlayInfos { get; set; }
        public DbSet<PullInfo> PullInfos { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<OperationInfo> OperationInfos { get;set; }

        public LiveFactoryDbContext(DbContextOptions options) : base(options)
        {
        }

    }
}
