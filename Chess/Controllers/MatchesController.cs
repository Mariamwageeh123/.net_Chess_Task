using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly ChessLeagueContext _context;

        public MatchesController(ChessLeagueContext context)
        {
            _context = context;
        }

        // POST: api/Matches/Generate
        [HttpPost("Generate")]
        public async Task<IActionResult> GenerateMatches()
        {
            var participants = await _context.Participants.ToListAsync();

            if (participants.Count < 2)
            {
                return BadRequest(new { message = "Not enough participants to create matches." });
            }

            var shuffledParticipants = participants.OrderBy(p => Guid.NewGuid()).ToList();
            int totalMatches = shuffledParticipants.Count / 2;

            var matches = new List<Match>();
            DateTime today = DateTime.Today;

            int maxMatchesPerDay = 3;
            int matchesCreated = 0;
            DateTime matchDate = today;

            while (matchesCreated < totalMatches)
            {
                int dailyMatches = 0;
                while (dailyMatches < maxMatchesPerDay && matchesCreated < totalMatches)
                {
                    var player1 = shuffledParticipants[0];
                    var player2 = shuffledParticipants[1];
                    shuffledParticipants.RemoveAt(0);
                    shuffledParticipants.RemoveAt(0);

                    var match = new Match
                    {
                        Player1Id = player1.ParticipantId,
                        Player2Id = player2.ParticipantId,
                        MatchTime = matchDate.AddHours(10 + dailyMatches * 2),
                        RoundNumber = 1
                    };
                    matches.Add(match);
                    matchesCreated++;
                    dailyMatches++;
                }

                matchDate = matchDate.AddDays(1);
            }

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Matches generated successfully.", data = matches });
        }

        // PUT: api/Matches/{id}/Result
        [HttpPut("{id}/Result")]
        public async Task<IActionResult> UpdateMatchWinner(int id, [FromBody] int result)
        {
            if (result < 0 || result > 2)
            {
                return BadRequest(new { message = "Invalid result. Use 0 for draw, 1 for Player 1 win, or 2 for Player 2 win." });
            }

            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound(new { message = "Match not found." });
            }

            match.Result = result;
            match.WinnerId = result switch
            {
                1 => match.Player1Id,
                2 => match.Player2Id,
                _ => null
            };

            _context.Entry(match).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Match result updated successfully." });
        }

        // GET: api/Matches/Daily
        [HttpGet("Daily")]
        public async Task<IActionResult> GetTodayMatches()
        {
            var today = DateTime.Today;
            var matches = await _context.Matches
                .Where(m => m.MatchTime.Date == today)
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .ToListAsync();

            if (matches == null || matches.Count == 0)
            {
                return Ok(new { message = "No matches scheduled for today." });
            }

            return Ok(new { message = "Today's matches retrieved successfully.", data = matches });
        }

        // GET: api/Matches/Scheduled
        [HttpGet("Scheduled")]
        public async Task<IActionResult> GetScheduledMatches()
        {
            var matches = await _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .OrderBy(m => m.MatchTime)
                .ToListAsync();

            if (matches == null || matches.Count == 0)
            {
                return Ok(new { message = "No scheduled matches found." });
            }

            return Ok(new { message = "Scheduled matches retrieved successfully.", data = matches });
        }

        // GET: api/Matches/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatch(int id)
        {
            var match = await _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null)
            {
                return NotFound(new { message = "Match not found." });
            }

            return Ok(new { message = "Match retrieved successfully.", data = match });
        }
    }
}
