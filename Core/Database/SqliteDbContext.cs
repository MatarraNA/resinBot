using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace resinBot.Core.Database
{
    public class SqliteDbContext : DbContext
    {
        public DbSet<DiscordUser> DiscordUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=Database.sqlite");
    }
}
