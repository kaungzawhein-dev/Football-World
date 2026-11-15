using FootballWorld.Data;
using FootballWorld.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballWorld.Services
{
    public class FootballService
    {
        private readonly FootballDbContext _context;

        public FootballService(FootballDbContext context)
        {
            _context = context;
        }

        public async Task<LeagueListResponseModel> GetDataByPagination(
            int pageNo = 1,
            int pageSize = 10,
            string? category = null)
        {
            LeagueListResponseModel response = new();

            pageNo = Math.Max(1, pageNo);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Leagues.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(x =>
                    x.Country.Contains(category) ||
                    x.Name.Contains(category));
            }

            int totalCount = await query.CountAsync();

            int pageCount = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            var pageSetting = new PaginationModel
            {
                PageNo = pageNo,
                PageSize = pageSize,
                PageCount = pageCount
            };

            response = new LeagueListResponseModel
            {
                Data = await GetLeagueByPagination(
                    pageNo, pageSize, category),

                PageSetting = pageSetting
            };

            return response;
        }

        public async Task<LeagueCategoryResponseModel> GetCategoryList()
        {
            var model = new LeagueCategoryResponseModel();

            var data = await _context.Leagues
                .AsNoTracking()
                .Select(league => new LeagueCategoryModel
                {
                    Id = league.Id,
                    Name = league.Name,
                    Country = league.Country,
                    Count = league.Teams.Count()
                })
                .OrderBy(x => x.Name)
                .ToListAsync();

            model.Data = data;

            return model;
        }

        public async Task<TeamListResponseModel> GetTeamsByLeague(
            int leagueId,
            int pageNo = 1,
            int pageSize = 10)
        {
            TeamListResponseModel response = new();

            pageNo = Math.Max(1, pageNo);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Teams
                .AsNoTracking()
                .Where(x => x.LeagueId == leagueId);

            int totalCount = await query.CountAsync();

            int pageCount = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            var pageSetting = new PaginationModel
            {
                PageNo = pageNo,
                PageSize = pageSize,
                PageCount = pageCount
            };

            response = new TeamListResponseModel
            {
                Data = await GetTeamsByPagination(
                    pageNo, pageSize, leagueId),

                PageSetting = pageSetting
            };

            return response;
        }

        public async Task<LeagueModel?> GetLeagueById(int id)
        {
            return await _context.Leagues
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new LeagueModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Country = x.Country,
                    LogoUrl = x.LogoUrl
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TeamModel?> GetTeamById(int id)
        {
            return await _context.Teams
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new TeamModel
                {
                    Id = x.Id,
                    LeagueId = x.LeagueId,
                    LeagueName = x.League != null
                        ? x.League.Name : string.Empty,
                    Name = x.Name,
                    Country = x.Country,
                    FoundedYear = x.FoundedYear,
                    Stadium = x.Stadium,
                    Manager = x.Manager,
                    LogoUrl = x.LogoUrl,
                    Description = x.Description
                })
                .FirstOrDefaultAsync();
        }

        private async Task<List<LeagueModel>> GetLeagueByPagination(
            int pageNo,
            int pageSize,
            string? category)
        {
            var query = _context.Leagues.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(x =>
                    x.Country.Contains(category) ||
                    x.Name.Contains(category));
            }

            var dataList = await query
                .OrderBy(x => x.Name)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new LeagueModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Country = x.Country,
                    LogoUrl = x.LogoUrl
                })
                .ToListAsync();

            return dataList;
        }

        private async Task<List<TeamModel>> GetTeamsByPagination(
            int pageNo,
            int pageSize,
            int leagueId)
        {
            var dataList = await _context.Teams
                .AsNoTracking()
                .Where(x => x.LeagueId == leagueId)
                .OrderBy(x => x.Name)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TeamModel
                {
                    Id = x.Id,
                    LeagueId = x.LeagueId,
                    LeagueName = x.League != null
                        ? x.League.Name : string.Empty,
                    Name = x.Name,
                    Country = x.Country,
                    FoundedYear = x.FoundedYear,
                    Stadium = x.Stadium,
                    Manager = x.Manager,
                    LogoUrl = x.LogoUrl,
                    Description = x.Description
                })
                .ToListAsync();

            return dataList;
        }
    }
}