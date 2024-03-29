﻿using exp.NET6.Services.DBServices;
using Microsoft.EntityFrameworkCore;
using Parkside.Infrastructure.Entities;
using Parkside.Infrastructure.Repositories.ChampionShips;
using Parkside.Infrastructure.Repositories.Matches;
using Parkside.Infrastructure.Repositories.Teams;
using Parkside.Models.Helpers;
using Parkside.Models.ViewModels;
using Parkside.Services.Matches;
using System.Text.RegularExpressions;

namespace Parkside.Services.Matchs
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepo _matchRepo;
        private readonly ITeamRepo _teamRepo;
        private readonly IChampionshipRepo _championshipRepo;
        private readonly IGenericService _genericService;
        public MatchService(IMatchRepo matchRepo,
            ITeamRepo teamRepo,
            IChampionshipRepo championshipRepo,
            IGenericService genericService)
        {
            _matchRepo = matchRepo;
            _teamRepo = teamRepo;
            _championshipRepo = championshipRepo;
            _genericService = genericService;
        }

        public async Task<MatchDetailsViewModel> GetMatch(int id)
        {
            var match = await _matchRepo.GetAllMatches().FirstOrDefaultAsync(x => x.Id == id);

            if (match == null)
                throw new NotFoundException("Match not found!");

            var finalMatch = new MatchDetailsViewModel
            {
                Id = match.Id,
                Location = match.Location,
                MatchDate = match.MatchDate,
                MatchHour = match.MatchHour,
                PlayingHome = match.PlayingHome,
                IsFinished = match.IsFinished,
                EnemyTeamPoints = match.EnemyTeamPoints,
                MainTeamPoints = match.MainTeamPoints,
                Championship = new ChampionshipViewModel
                {
                    Id = match.ChampionshipId,
                    Name = match.Championship.Name
                },
                EnemyTeam = new TeamViewModel
                {
                    Id = match.EnemyTeamId,
                    Name = match.EnemyTeam.Name
                }
            };

            return finalMatch;
        }

        public PagingViewModel<MatchViewModel> GetMatches(
            string? NameSearch, string? OrderBy, string? MatchDate, int PageNumber, int PageSize)
        {
            var matches = _matchRepo.GetAllMatches();

            if (!string.IsNullOrWhiteSpace(NameSearch))
            {
                NameSearch = NameSearch.Trim();
                matches = matches.Where(c => c.EnemyTeam.Name.Contains(NameSearch));
            }

            if (!string.IsNullOrWhiteSpace(MatchDate))
            {
                MatchDate = MatchDate.Trim();

                if (DateTime.TryParse(MatchDate, out var matchDate))
                {
                    matches = matches.Where(n => n.MatchDate != null && n.MatchDate.Value.Date == matchDate.Date);
                }
            }


            switch (OrderBy)
            {
                case ("matchdate"):
                    matches = matches.OrderBy(c => c.MatchDate);
                    break;

                case ("matchdate_desc"):
                    matches = matches.OrderByDescending(c => c.MatchDate);
                    break;

                default:
                    matches = matches.OrderByDescending(c => c.MatchDate);
                    break;
            }

            var numberOfMatchs = matches.Count();

            var matchessPerPage = matches.Skip(PageSize * (PageNumber - 1))
              .Take(PageSize).Select(match => new MatchViewModel
              {
                  Id = match.Id,
                  ChampionshipName = match.Championship.Name,
                  EnemyTeamName = match.EnemyTeam.Name,
                  EnemyTeamImageBase64 = _genericService.GetImgBase64(match.EnemyTeam.ImageUrl),
                  ChampionshipImageBase64 = _genericService.GetImgBase64(match.Championship.ImageUrl),
                  Location = match.Location,
                  MatchDate = match.MatchDate.HasValue ? match.MatchDate.Value.ToString("dd/MM/yyyy") : null,
                  MatchHour = match.MatchHour,
                  PlayingHome = match.PlayingHome,
                  IsFinished = match.IsFinished,
                  EnemyTeamPoints = match.EnemyTeamPoints,
                  MainTeamPoints = match.MainTeamPoints
              })
              .ToList();

            var paginingList = new PagingViewModel<MatchViewModel>
            {
                Count = numberOfMatchs,
                NumberOfPages = (int)Math.Ceiling(numberOfMatchs / (double)PageSize),
                Items = matchessPerPage
            };

            return paginingList;
        }

        public async Task AddMatch(int enemyTeamId, int championshipId, MatchCreateViewModel model)
        {
            if (await _teamRepo.GetAsync(enemyTeamId) == null)
            {
                throw new Exception("Enemy team not found!");
            }
            if (await _championshipRepo.GetAsync(championshipId) == null)
            {
                throw new Exception("Championship not found!");
            }

            var finalMatch = new Infrastructure.Entities.Match()
            {
                ChampionshipId = championshipId,
                EnemyTeamId = enemyTeamId,
                Location = model.Location,
                MatchDate = model.MatchDate,
                MatchHour = model.MatchHour,
                PlayingHome = model.PlayingHome,
                IsFinished = model.IsFinished,
                EnemyTeamPoints = model.EnemyTeamPoints,
                MainTeamPoints = model.MainTeamPoints,
            };

            await _matchRepo.Add(finalMatch);
        }
        public async Task DeleteMatch(int id)
        {
            var match = await _matchRepo.GetAsync(id);
            if (match == null)
                throw new NotFoundException("Match not found!");

            await _matchRepo.Delete(id);
        }

        public async Task VirtualDeleteMatch(int id)
        {
            var match = await _matchRepo.GetAsync(id);
            if (match == null)
                throw new NotFoundException("Match not found!");

            match.IsDeleted = true;

            await _matchRepo.Update(match);
        }

        public async Task UpdateMatch(int matchId, int enemyTeamId, int championshipId, MatchUpdateViewModel model)
        {
            var match = await _matchRepo.GetAsync(matchId);
            if (match == null)
                throw new NotFoundException("Match not found!");

            match.ChampionshipId = championshipId;
            match.EnemyTeamId = enemyTeamId;
            match.Location = model.Location;
            match.MatchDate = model.MatchDate;
            match.MatchHour = model.MatchHour;
            match.PlayingHome = model.PlayingHome;
            match.IsFinished = model.IsFinished;
            match.EnemyTeamPoints = model.EnemyTeamPoints;
            match.MainTeamPoints = model.MainTeamPoints;

            await _matchRepo.Update(match);
        }

        public IQueryable<MatchViewModel> GetHomePageMatches()
        {
            var matches = _matchRepo.GetAllMatches().OrderBy(c => c.MatchDate);

            var finalMatchs = matches.Select(match => new MatchViewModel
            {
                Id = match.Id,
                ChampionshipName = match.Championship.Name,
                EnemyTeamName = match.EnemyTeam.Name,
                EnemyTeamImageBase64 = _genericService.GetImgBase64(match.EnemyTeam.ImageUrl),
                ChampionshipImageBase64 = _genericService.GetImgBase64(match.Championship.ImageUrl),
                Location = match.Location,
                MatchDate = match.MatchDate.HasValue ? match.MatchDate.Value.ToString("dd/MM/yyyy") : null,
                MatchHour = match.MatchHour,
                PlayingHome = match.PlayingHome,
                IsFinished = match.IsFinished,
                EnemyTeamPoints = match.EnemyTeamPoints,
                MainTeamPoints = match.MainTeamPoints
            });

            return finalMatchs;
        }
    }
}

