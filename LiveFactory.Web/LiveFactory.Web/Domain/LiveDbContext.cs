using LiveFactory.Web.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;

namespace LiveFactory.Web.Domain
{
    public class LiveDbContext : DbContext
    {
        public DbSet<LiveChannel> LiveChannels { get; set; }
        public DbSet<Users> User { get; set; }
        public LiveDbContext()
        {

        }
        public LiveDbContext(DbContextOptions<LiveDbContext> options):base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LiveChannel>().Property(s => s.Name).HasMaxLength(256);
            modelBuilder.Entity<LiveChannel>().Property(s => s.Url).HasMaxLength(513);
            modelBuilder.Entity<LiveChannel>().Property(s => s.Image).HasMaxLength(513);
            modelBuilder.Entity<LiveChannel>().Property(s => s.VideoType).HasMaxLength(4);
            modelBuilder.Entity<Users>().Property(s => s.Account).HasMaxLength(256);
            modelBuilder.Entity<Users>().Property(s => s.Password).HasMaxLength(256);
            modelBuilder.Entity<Users>().Property(s => s.LastTime).HasMaxLength(256);
            modelBuilder.Entity<Users>().Property(s => s.LastIP).HasMaxLength(513);
            modelBuilder.Entity<Users>().Property(s => s.IsDisabled).HasMaxLength(128);
        }

    }

    /// <summary>
    /// 直播频道
    /// </summary>
    public class LiveChannel
    {
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 直播地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }

        public VideoType.PlayerType VideoType { get; set; }
    }

    /// <summary>
    /// 用户
    /// </summary>
    public class Users
    {
        public int Id { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 最后登陆IP
        /// </summary>
        public string LastIP { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled { get; set; } = false;
    }
}
