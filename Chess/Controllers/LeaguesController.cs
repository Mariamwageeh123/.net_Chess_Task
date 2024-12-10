using Chess.Dtos;
using Chess.Models;
using Chess.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Macs;

namespace Chess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaguesController : ControllerBase
    {
        private readonly ChessLeagueContext _context;

        public LeaguesController(ChessLeagueContext context)
        {
            _context = context;
        }

        // GET: api/Leagues
        [HttpGet("GetActiveLeagues")]
        public async Task<ActionResult<IEnumerable<League>>> GetActiveLeagues()
        {
            return await _context.Leagues.Where(L => L.IsActive == true).ToListAsync();
        }
        // GET: api/Leagues
        [HttpGet("GetAllLeagues")]
        public async Task<ActionResult<IEnumerable<League>>> GetAllLeagues()
        {
            return await _context.Leagues.ToListAsync();
        }

        // POST: api/Leagues
        [HttpPost]
        public async Task<ActionResult<League>> CreateLeague([FromForm] AddLeageuDto Dto)
        {

            var league = new League
            {
                LeagueName = Dto.LeagueName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            _context.Leagues.Add(league);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLeague), new { id = league.LeagueId }, league);
        }

        // GET: api/Leagues/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<League>> GetLeague(int id)
        {
            var league = await _context.Leagues
                .Include(l => l.LeagueParticipants)
                .FirstOrDefaultAsync(l => l.LeagueId == id);

            if (league == null)
            {
                return NotFound(new { message = "League not found." });
            }


            var leagueDto = new LeagueDto
            {
                LeagueId = league.LeagueId,
                LeagueParticipants = league.LeagueParticipants.Select(lp => new LeagueParticipantDto
                {
                    ParticipantId = lp.ParticipantId,
                    LeagueId = lp.LeagueId
                }).ToList()
            };



            return Ok(new { message = "League retrieved successfully.", data = leagueDto });

        }

        // PUT: api/Leagues/{id}/Close
        [HttpPut("{id}/Close")]
        public async Task<IActionResult> CloseLeague(int id)
        {
            var league = await _context.Leagues.FindAsync(id);
            if (league == null)
            {
                return NotFound();
            }

            league.IsActive = false;
            _context.Leagues.Update(league);
            await _context.SaveChangesAsync();

            return Ok(league);
        }
        // put: api/leagues/{id}/close
        [HttpPost("{leagueid}/{winnerid}/close")]
        public async Task<IActionResult> Setleaguewinner(int leagueid, int winnerid)
        {

            var winner = await _context.Participants.FindAsync(winnerid);
            var league = await _context.Leagues.FindAsync(leagueid);
            if (winner == null && league == null) return NotFound();

            var email = new CongratulationMail
            {
                ChampionId = winnerid,
                LeagueId = leagueid,
                EmailContent = "Conglatulation you are win ",
                SentAt = DateTime.Now
            };

            EmailService.SendEmailAsync(winner.Email, "congratulations", $"you are the winner of the league tournament : {league.LeagueName}");

            league.WennerName = winner.Name;

            _context.Leagues.Update(league);
            await _context.SaveChangesAsync();

            return Ok(league);
        }
    }
}
