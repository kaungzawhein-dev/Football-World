
using FootballWorld.Data;
using FootballWorld.Models;
using FootballWorld.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballWorld.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly FootballDbContext _context;
        private readonly FootballService _service;

        public TeamsController(
            FootballDbContext context,
            FootballService service)
        {
            _context = context;
            _service = service;
        }

        // GET: api/teams?leagueId=1
        [HttpGet]
        public async Task<IActionResult> GetAll(
            int leagueId,
            int pageNo = 1,
            int pageSize = 10)
        {
            var data = await _service.GetTeamsByLeague(
                leagueId, pageNo, pageSize);

            return Ok(data);
        }

        // GET: api/teams/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetTeamById(id);

            if (data == null)
                return NotFound("Team not found.");

            return Ok(data);
        }

        // POST: api/teams
        [HttpPost]
        public async Task<IActionResult> Create(Team model)
        {
            model.Id = 0;
            model.League = null;

            bool leagueExists = await _context.Leagues
                .AnyAsync(x => x.Id == model.LeagueId);

            if (!leagueExists)
                return BadRequest("Invalid LeagueId.");

            _context.Teams.Add(model);

            await _context.SaveChangesAsync();

            var result = await _service.GetTeamById(model.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = model.Id },
                result);
        }

        // PUT: api/teams/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id, Team model)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
                return NotFound("Team not found.");

            bool leagueExists = await _context.Leagues
                .AnyAsync(x => x.Id == model.LeagueId);

            if (!leagueExists)
                return BadRequest("Invalid LeagueId.");

            team.LeagueId = model.LeagueId;
            team.Name = model.Name;
            team.Country = model.Country;
            team.FoundedYear = model.FoundedYear;
            team.Stadium = model.Stadium;
            team.Manager = model.Manager;
            team.LogoUrl = model.LogoUrl;
            team.Description = model.Description;

            await _context.SaveChangesAsync();

            return Ok(await _service.GetTeamById(id));
        }

        // DELETE: api/teams/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
                return NotFound("Team not found.");

            _context.Teams.Remove(team);

            await _context.SaveChangesAsync();

            return Ok("Team deleted successfully.");
        }
    }
}
