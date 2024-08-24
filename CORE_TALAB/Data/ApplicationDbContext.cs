using Microsoft.EntityFrameworkCore;
using System;

namespace CORE_TALAB.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
              : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // để có thể lưu được thời gian thì cần phải khai báo này
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }
        public DbSet<CORE_TALAB.Models.Table.Warrantys> warrantys { get; set; }
        public DbSet<CORE_TALAB.Models.Table.images> images { get; set; }
        public DbSet<CORE_TALAB.Models.Table.auth> auth { get; set; }
    }
}
