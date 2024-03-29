﻿using Microsoft.EntityFrameworkCore;
using Parkside.Infrastructure.Entities;

namespace Parkside.Infrastructure.Context
{
    public partial class ParksideContext : DbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Sponsor>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<SocialMedia>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<News>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Stuff>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<StuffHistory>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Championship>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Match>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Team>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Trofee>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<PlayersHistory>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<PlayersTrofee>().HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
