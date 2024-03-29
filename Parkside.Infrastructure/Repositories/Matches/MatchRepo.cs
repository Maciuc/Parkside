﻿using Microsoft.EntityFrameworkCore;
using Parkside.Infrastructure.Context;
using Parkside.Infrastructure.Entities;
using Parkside.Infrastructure.Repositories.Generic;

namespace Parkside.Infrastructure.Repositories.Matches
{
    public class MatchRepo : GenericRepository<Match>, IMatchRepo
    {
        public ParksideContext _parksideContext { get; set; }
        public MatchRepo(ParksideContext context) : base(context)
        {
            _parksideContext = context;
        }

        public IQueryable<Match> GetAllMatches()
        {
            var matches = _parksideContext.Matches.Where(x => x.IsDeleted != true).Include(x => x.Championship).IgnoreQueryFilters().Include(y => y.EnemyTeam).IgnoreQueryFilters();
            return matches;
        }

    }
}
